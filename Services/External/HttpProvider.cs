using System.Text;

namespace Services.External
{
    public class HttpProvider : IHttpProvider
    {
        private static readonly HttpClient _client = new HttpClient();

        // Todo: Inject from Program.cs

        public async Task<string> Get(string url)
        {
            var responseTask = _client.GetAsync(url);
            await responseTask;
            var result = await responseTask;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                await readTask;
                var resultJson = await readTask;

                return resultJson;
            }

            return "";
        }

        public async Task<string> Post(string url, string json)
        {
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var responseTask = _client.PostAsync(url, requestContent);
            await responseTask;
            var result = await responseTask;
            if (result.IsSuccessStatusCode)
            {
                var resultBack = await result.Content.ReadAsStringAsync();
                var resultBackJson = resultBack;
                return resultBack;
            }

            return "";
        }
    }

}