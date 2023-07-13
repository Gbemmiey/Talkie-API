namespace Talkie.Services.GenericServices
{
    public interface IGenericService
    {
        Task<decimal> GetUserBalance();

        string GetUserID();
    }
}