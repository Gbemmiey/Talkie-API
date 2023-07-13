namespace Talkie.Services.TransactionService
{
    public interface ITransactionService
    {
        Task transferMoneyAsync(string recipientAccountNumber, decimal amount);

        Task<decimal> GetUserBalance();

        Task DebitUserAsync(decimal amount);
    }
}