namespace CommerceOrders.Services.External
{
    public interface IHttpProvider
    {
        Task<string> Get(string url);

        Task<string> Post(string url, string json);
    }
}