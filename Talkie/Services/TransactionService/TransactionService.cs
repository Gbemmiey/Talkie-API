using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Talkie.Controllers;
using Talkie.Data;
using Talkie.DTOs.Message;
using Talkie.Models;
using Talkie.Services.Auth;
using Talkie.Services.GenericServices;

namespace Talkie.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericService _genericService;
        private readonly IAuthRepository _authRepository;

        public TransactionService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IGenericService genericService, IAuthRepository authRepository)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _genericService = genericService;
            _authRepository = authRepository;
        }

        private async Task<decimal> GetUserBalance()
        {
            decimal balance;
            var account = await _context.Accounts.FirstAsync(c => c.Number == _genericService.GetUserID());
            balance = account.Balance;

            return balance;
        }

        private bool SufficientFunds(decimal transactionAmount)
        {
            Task<decimal> task = _genericService.GetUserBalance();

            var awaiter = task.GetAwaiter();
            decimal balance = awaiter.GetResult();

            if (balance > transactionAmount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task transferMoneyAsync(string recipientAccountNumber, decimal amount)
        {
            if (SufficientFunds(amount))
            {
                await DebitUserAsync(amount);
                await CreditFriendAsync(recipientAccountNumber, amount);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Insufficient Funds");
            }
        }

        private async Task DebitUserAsync(decimal amount)
        {
            var account = await _context.Accounts.FirstAsync(c => c.Number == _genericService.GetUserID());
            account.Balance = account.Balance - amount;

            _context.Add(account);

            System.Diagnostics.Debug.WriteLine(account.Balance);
            System.Diagnostics.Debug.WriteLine(account);
            _context.Update(account);
            _context.SaveChanges();
        }

        private async Task CreditFriendAsync(string accountNumber, decimal amount)
        {
            var account = await _context.Accounts.FirstAsync(c => c.Number == accountNumber);
            account.Balance = account.Balance + amount;

            _context.Add(account);

            System.Diagnostics.Debug.WriteLine(account.Balance);
            System.Diagnostics.Debug.WriteLine(account);

            _context.Update(account);
            _context.SaveChanges();
        }

        public async Task<ServiceResponse<GetMessageDto>> SendMoney(AddMessageDto newMessage)
        {
            var serviceResponse = new ServiceResponse<GetMessageDto>();

            DateTime wcaTime = _genericService.getLocalTime();

            var jsonString = JsonConvert.SerializeObject(newMessage.Payload);

            Account? act = await _context.Accounts.Where(c => c.Number == _genericService.GetUserID()).FirstOrDefaultAsync();

            if (_authRepository.VerifyPinHash(newMessage.AuthPin, act.PinHash, act.PinSalt))
            {
                Transaction newTran = _mapper.Map<Models.Transaction>(jsonString);

                Message newMess = new Message
                {
                    Modified = wcaTime,
                    RecipientNumber = newMessage.RecipientNumber,
                    Type = newMessage.Type,
                    Number = _genericService.GetUserID(),
                    DeliveryStatus = DeliveryStatus.Sent
                };

                Transaction nT = new Transaction
                {
                    Amount = Convert.ToDecimal(newMessage.Payload)
                };

                nT.Message = newMess;

                _context.Transactions.Add(nT);
                await _context.SaveChangesAsync();

                // Perform transaction
                await transferMoneyAsync(newMess.RecipientNumber, nT.Amount);

                await getSentMoney(serviceResponse, nT);
                return serviceResponse;
            }
            else
            {
                serviceResponse.Message = "Incorrect Pin";
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }

        private async Task getSentMoney(ServiceResponse<GetMessageDto> serviceResponse, Transaction nT)
        {
            Message? saveMessage = await _context.Messages
                                            .Include(c => c.Transactions)
                                            .FirstOrDefaultAsync(c => c.Id == nT.MessageId);

            Transaction? savedTran = await _context.Transactions
                                            .FirstOrDefaultAsync(c => c.MessageId == nT.MessageId);

            GetMessageDto responseData = _mapper.Map<GetMessageDto>(saveMessage);
            responseData.Payload = Convert.ToString(savedTran.Amount);
            responseData.Interaction = "Sent";

            serviceResponse.Data = _mapper.Map<GetMessageDto>(responseData);
        }
    }
}