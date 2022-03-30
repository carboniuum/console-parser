using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleParser.Core
{
    public static class Settings
    {
        public static string BaseUrl { get; private set; } = "https://jut.su";

        public static string UserAgent { get; private set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.84 Safari/537.36";

        public static async Task<Cookie> GetCookie()
        {
            var cookieContainer = new CookieContainer();
            var uri = new Uri(BaseUrl);
            using (var httpClientHandler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    await httpClient.GetAsync(uri);
                    return cookieContainer.GetCookies(uri).Cast<Cookie>().ToList().First();
                }
            }
        }
    }
}
