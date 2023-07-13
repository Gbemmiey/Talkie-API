using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talkie.Data;
using Talkie.Services.GenericServices;

namespace Talkie.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericService _genericService;

        public TransactionService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IGenericService genericService)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _genericService = genericService;
        }

        public async Task<decimal> GetUserBalance()
        {
            decimal balance;
            var account = await _context.Accounts.FirstAsync(c => c.Number == _genericService.GetUserID());
            balance = account.Balance;

            return balance;
        }

        public bool SufficientFunds(decimal transactionAmount)
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

        public async Task transferMoneyAsync(string recipientAccountNumber, decimal amount)
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

        public async Task DebitUserAsync(decimal amount)
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
    }
}