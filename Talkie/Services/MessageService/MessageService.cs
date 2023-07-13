using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using Talkie.Controllers;
using Talkie.Data;
using Talkie.DTOs.Account;
using Talkie.DTOs.Message;
using Talkie.Models;
using Talkie.Services.Auth;
using Talkie.Services.GenericServices;
using Talkie.Services.TransactionService;
using Transaction = Talkie.Models.Transaction;

namespace Talkie.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITransactionService _transact;
        private readonly IGenericService _genericService;
        private readonly IAuthRepository _authRepository;

        private decimal value;

        public MessageService(IMapper mapper, DataContext context,
            IHttpContextAccessor httpContextAccessor, ITransactionService transact,
            IGenericService genericService, IAuthRepository authRepository)
        {
            _mapper = mapper;
            _transact = transact;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _genericService = genericService;
            _authRepository = authRepository;
        }

        public async Task<ServiceResponse<GetMessageDto>> AddMessage(AddMessageDto newMessage)
        {
            var serviceResponse = new ServiceResponse<GetMessageDto>();

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo wcaZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            // Convert the UTC time to West Central Africa time
            DateTime wcaTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, wcaZone);

            var jsonString = JsonConvert.SerializeObject(newMessage.Payload);

            if (newMessage.Type == MessageType.Text)
            {
                Text newText = _mapper.Map<Text>(jsonString);

                Message newMess = new Message
                {
                    Modified = wcaTime,
                    RecipientNumber = newMessage.RecipientNumber,
                    Type = newMessage.Type,
                    Number = _genericService.GetUserID()
                };

                Text nT = new Text
                {
                    Content = newMessage.Payload
                };

                nT.Message = newMess;

                _context.Texts.Add(nT);

                await _context.SaveChangesAsync();

                Message? saveMessage = await _context.Messages
                                            .Include(c => c.Texts)
                                            .FirstOrDefaultAsync(c => c.Id == nT.MessageId);

                Text? savedText = await _context.Texts
                                                .FirstOrDefaultAsync(c => c.MessageId == nT.MessageId);

                GetMessageDto responseData = _mapper.Map<GetMessageDto>(saveMessage);

                responseData.Payload = savedText.Content;
                responseData.Interaction = "Sent";

                serviceResponse.Data = _mapper.Map<GetMessageDto>(responseData);
                return serviceResponse;
            }
            else if (newMessage.Type == MessageType.Transaction)
            {
                Account? act = await _context.Accounts.Where(c => c.Number == _genericService.GetUserID()).FirstOrDefaultAsync();

                if (_authRepository.VerifyPinHash(newMessage.AuthPin, act.PinHash, act.PinSalt))
                {
                    Transaction newTran = _mapper.Map<Models.Transaction>(jsonString);

                    Message newMess = new Message
                    {
                        Modified = wcaTime,
                        RecipientNumber = newMessage.RecipientNumber,
                        Type = newMessage.Type,
                        Number = _genericService.GetUserID()
                    };

                    Transaction nT = new Transaction
                    {
                        Amount = Convert.ToDecimal(newMessage.Payload)
                    };

                    nT.Message = newMess;

                    _context.Transactions.Add(nT);
                    await _context.SaveChangesAsync();

                    // Perform transaction
                    await _transact.transferMoneyAsync(newMess.RecipientNumber, nT.Amount);

                    Message? saveMessage = await _context.Messages
                                                .Include(c => c.Transactions)
                                                .FirstOrDefaultAsync(c => c.Id == nT.MessageId);

                    Transaction? savedTran = await _context.Transactions
                                                    .FirstOrDefaultAsync(c => c.MessageId == nT.MessageId);

                    GetMessageDto responseData = _mapper.Map<GetMessageDto>(saveMessage);
                    responseData.Payload = Convert.ToString(savedTran.Amount);
                    responseData.Interaction = "Sent";

                    serviceResponse.Data = _mapper.Map<GetMessageDto>(responseData);
                    return serviceResponse;
                }
            }
            else if (newMessage.Type == MessageType.File)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Unimplemented";
            }
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Unable to determine message type";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetMessageDto>>> GetConversation(string contact)
        {
            var serviceResponse = new ServiceResponse<List<GetMessageDto>>();

            var sent = await SentMessages(contact);
            var received = await ReceivedMessages(contact);

            var total = sent.Concat(received);

            var sortedTotalByModified = total.OrderBy(d => d.Modified);

            var newList = sortedTotalByModified.Skip(sortedTotalByModified.Count() - 10).ToList();

            serviceResponse.Data = total.ToList();

            serviceResponse.Data = sortedTotalByModified.ToList();
            serviceResponse.Data = newList;

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetMessageDto>>> GetSentMessages(string contact)
        {
            var serviceResponse = new ServiceResponse<List<GetMessageDto>>();

            List<GetMessageDto> response = await SentMessages(contact);

            serviceResponse.Data = response;
            return serviceResponse;
        }

        private async Task<List<GetMessageDto>> SentMessages(string friendNumber)
        {
            var serviceResponse = new ServiceResponse<List<GetMessageDto>>();

            var newMess = await _context.Messages
                                    .Where(c => c.Number == _genericService.GetUserID() && c.RecipientNumber == friendNumber)
                                    .Include(c => c.Texts)
                                    .Include(c => c.Transactions)
                                    .ToListAsync();

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.CurrencySymbol = "₦";

            List<GetMessageDto>? myList = new List<GetMessageDto>();

            foreach (var message in newMess)
            {
                GetMessageDto messageRecord = new GetMessageDto
                {
                    Id = message.Id,
                    Modified = message.Modified,
                    Type = message.Type,
                    RecipientNumber = message.RecipientNumber,
                    Interaction = "Sent"
                };
                if (message.Type == MessageType.Text)
                {
                    messageRecord.Payload = message.Texts.FirstOrDefault().Content;
                }
                else
                {
                    messageRecord.Payload = "You sent " + message.Transactions.FirstOrDefault().Amount.ToString("C", nfi);
                }
                myList.Add(messageRecord);
            }

            serviceResponse.Data = myList;
            serviceResponse.Success = true;

            return serviceResponse.Data;
        }

        public async Task<ServiceResponse<List<GetMessageDto>>> GetReceivedMessages(string contact)
        {
            var serviceResponse = new ServiceResponse<List<GetMessageDto>>();
            List<GetMessageDto> response = await ReceivedMessages(contact);

            serviceResponse.Data = response;
            return serviceResponse;
        }

        private async Task<List<GetMessageDto>> ReceivedMessages(string friendNumber)
        {
            var serviceResponse = new ServiceResponse<List<GetMessageDto>>();

            var newMess = await _context.Messages
                                    .Where(c => c.RecipientNumber == _genericService.GetUserID() && c.Number == friendNumber)
                                    .Include(c => c.Texts)
                                    .Include(c => c.Transactions)
                                    .ToListAsync();

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.CurrencySymbol = "₦";

            List<GetMessageDto>? myList = new List<GetMessageDto>();

            foreach (var message in newMess)
            {
                GetMessageDto messageRecord = new GetMessageDto
                {
                    Id = message.Id,
                    Modified = message.Modified,
                    Type = message.Type,
                    RecipientNumber = message.RecipientNumber,
                    Interaction = "Received"
                };
                if (message.Type == MessageType.Text)
                {
                    messageRecord.Payload = message.Texts.FirstOrDefault().Content;
                }
                else
                {
                    messageRecord.Payload = "You received " + message.Transactions.FirstOrDefault().Amount.ToString("C", nfi);
                }
                myList.Add(messageRecord);
            }

            serviceResponse.Data = myList;
            serviceResponse.Success = true;

            return serviceResponse.Data;
        }

        public async Task<ServiceResponse<List<GetMessageDto>>> GetTransactions()
        {
            var serviceResponse = new ServiceResponse<List<GetMessageDto>>();

            var sent = await SentTransactions();
            var received = await ReceivedTransactions();

            var total = sent.Concat(received);

            var sortedTotalByModified = total.OrderBy(d => d.Modified);

            serviceResponse.Data = total.ToList();

            serviceResponse.Data = sortedTotalByModified.ToList();

            return serviceResponse;
        }

        private async Task<List<GetMessageDto>> SentTransactions()
        {
            List<Message> newMess = await _context.Messages
                                    .Where(c => c.Number == _genericService.GetUserID() && c.Type == MessageType.Transaction)
                                    .ToListAsync();

            var response = _mapper.Map<List<GetMessageDto>>(newMess);

            foreach (GetMessageDto item in response)
            {
                item.Payload = Convert.ToString(await _context.Transactions
                    .Where(c => c.MessageId == item.Id)
                    .Select(c => c.Amount)
                    .FirstAsync());
                item.Interaction = "Sent";
            }

            return response;
        }

        private async Task<List<GetMessageDto>> ReceivedTransactions()
        {
            List<Message> newMess = await _context.Messages
                                               .Where(c => c.RecipientNumber == _genericService.GetUserID() && c.Type == MessageType.Transaction)
                                               .ToListAsync();

            var response = _mapper.Map<List<GetMessageDto>>(newMess);

            foreach (GetMessageDto item in response)
            {
                item.Payload = Convert.ToString(await _context.Transactions
                    .Where(c => c.MessageId == item.Id)
                    .Select(c => c.Amount)
                    .FirstAsync());
                item.Interaction = "Received";
            }

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetMessageHistory()
        {
            var serviceResponse = new ServiceResponse<List<string>>();

            List<string> sent = await SentMessagesHistory();
            List<string> received = await ReceivedMessagesHistory();

            var total = sent.Concat(received).Distinct().ToList();

            serviceResponse.Data = total;

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetAccountDto>>> GetContactsHistory()
        {
            var serviceResponse = new ServiceResponse<List<GetAccountDto>>();

            List<string> sent = await SentMessagesHistory();
            List<string> received = await ReceivedMessagesHistory();

            var total = sent.Concat(received).Distinct().ToList();

            var accounts = _context.Accounts
                                    .Where(c => total.Contains(c.Number));

            List<GetAccountDto> contactHistory = _mapper.Map<List<GetAccountDto>>(accounts);

            serviceResponse.Data = contactHistory;

            return serviceResponse;
        }

        private async Task<List<string>> ReceivedMessagesHistory()
        {
            var newMess = await _context.Messages
                                        .Where(c => c.RecipientNumber == _genericService.GetUserID())
                                        .Select(c => c.Number)
                                        .Distinct()
                                        .ToListAsync();

            return newMess;
        }

        private async Task<List<string>> SentMessagesHistory()
        {
            var newMess = await _context.Messages
                                    .Where(c => c.Number == _genericService.GetUserID())
                                    .Select(c => c.RecipientNumber)
                                    .Distinct()
                                    .ToListAsync();
            return newMess;
        }

        public async Task<ServiceResponse<List<GetStatementDto>>> GetAllTransactions()
        {
            var serviceResponse = new ServiceResponse<List<GetStatementDto>>();

            var sent = await AllSentTransactions();
            var received = await AllReceivedTransactions();

            var total = sent.Concat(received);

            var sortedTotalByModified = total.OrderByDescending(d => d.Modified);

            serviceResponse.Data = total.ToList();

            serviceResponse.Data = sortedTotalByModified.ToList();

            return serviceResponse;
        }

        private async Task<List<GetStatementDto>> AllSentTransactions()
        {
            List<string> sent = await SentMessagesHistory();

            // List<long> received = await ReceivedMessagesHistory();

            //            var total = sent.Concat(received).Distinct().ToList();
            var total = sent.Distinct().ToList();

            var accounts = _context.Accounts
                                    .Where(c => total.Contains(c.Number));

            List<GetAccountDto> contactHistory = _mapper.Map<List<GetAccountDto>>(accounts);

            var newMess = await _context.Messages
                                                     .Include(c => c.Transactions)
                                                     .Where(c => c.Number == _genericService.GetUserID() && c.Type == MessageType.Transaction)
                                                     .ToListAsync();

            List<GetStatementDto>? myList = new List<GetStatementDto>();

            System.Diagnostics.Debug.WriteLine("Filtering");

            foreach (var message in newMess)
            {
                GetAccountDto filteredData = contactHistory.First(a => a.Number == message.RecipientNumber);

                GetStatementDto statementDto = new GetStatementDto
                {
                    Payload = Convert.ToString(message.Transactions.FirstOrDefault().Amount),
                    Interaction = "Sent",
                    ContactNumber = message.RecipientNumber,

                    FirstName = filteredData.FirstName,

                    LastName = filteredData.LastName,
                    ProfilePictureUrl = filteredData.ProfilePictureUrl,
                    Number = null,
                    RecipientNumber = null,
                    Id = message.Id,
                    Modified = message.Modified
                };
                myList.Add(statementDto);
            }

            var response = myList;

            return response;
        }

        private async Task<List<GetStatementDto>> AllReceivedTransactions()
        {
            List<string> received = await ReceivedMessagesHistory();
            var total = received.Distinct().ToList();
            var accounts = _context.Accounts
                                    .Where(c => total.Contains(c.Number));
            List<GetAccountDto> contactHistory = _mapper.Map<List<GetAccountDto>>(accounts);

            var newMess = await _context.Messages
                                         .Where(c => c.RecipientNumber == _genericService.GetUserID() && c.Type == MessageType.Transaction)
                                         .Include(c => c.Transactions)
                                         .ToListAsync();

            List<GetStatementDto>? myList = new List<GetStatementDto>();

            foreach (var message in newMess)
            {
                GetAccountDto filteredData = contactHistory.First(a => a.Number == message.Number);

                GetStatementDto statementDto = new GetStatementDto
                {
                    Payload = Convert.ToString(message.Transactions.FirstOrDefault().Amount),
                    Interaction = "Received",
                    ContactNumber = message.Number,
                    FirstName = filteredData.FirstName,
                    LastName = filteredData.LastName,
                    ProfilePictureUrl = filteredData.ProfilePictureUrl,
                    Number = null,
                    RecipientNumber = null,
                    Id = message.Id,
                    Modified = message.Modified
                };
                myList.Add(statementDto);
            }

            var response = myList;
            return response;
        }

        public Task<ServiceResponse<List<GetMessageDto>>> GetTransactions(string contact)
        {
            throw new NotImplementedException();
        }
    }
}