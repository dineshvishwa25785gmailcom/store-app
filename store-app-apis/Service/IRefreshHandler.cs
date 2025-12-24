namespace store_app_apis.Service
{
    public interface IRefreshHandler
    {
        Task<string> GenerateToken(string username);
    }
}
