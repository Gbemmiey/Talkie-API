namespace Talkie.Services.GenericServices
{
    public interface IGenericService
    {
        DateTime getLocalTime();

        Task<decimal> GetUserBalance();

        string GetUserID();
    }
}