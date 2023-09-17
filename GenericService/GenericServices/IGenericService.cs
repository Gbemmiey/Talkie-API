namespace GenericService
{
    public interface IGenericService
    {
        Task<decimal> GetUserBalance();

        string GetUserID();

        DateTime getLocalTime();
    }
}