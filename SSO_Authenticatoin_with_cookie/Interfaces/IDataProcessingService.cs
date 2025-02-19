namespace SSO_Authenticatoin_with_cookie.Interfaces
{
    public interface IDataProcessingService
    {
        Task<string> ProtectData(string data);

        Task<string> UnprotectData(string protectedData);
    }
}
