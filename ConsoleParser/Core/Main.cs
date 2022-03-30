using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConsoleParser.Extensions;

namespace ConsoleParser.Core
{
    public class Main
    {
        public HtmlParser Parser { get; private set; }

        public Main()
        {
            Parser = new HtmlParser();
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string query)
        {
            var baseAddress = new Uri(Settings.BaseUrl);
            var cookieContainer = new CookieContainer();
            var sb = new StringBuilder();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);
                    var body = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("makeme", "yes"),
                        new KeyValuePair<string, string>("ystext", query)
                    });
                    cookieContainer.Add(baseAddress, await Settings.GetCookie());
                    var result = await client.PostAsync("/search", body);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    var document = await Parser.ParseDocumentAsync(resultContent);
                    var episodeLinks = document.QuerySelectorAll(".content a");
                    var episodes = new Dictionary<string, string>();

                    foreach (var ep in episodeLinks)
                    {
                        episodes.TryAdd(
                            $"{Settings.BaseUrl + ep.GetAttribute("href")}", 
                            ep.TextContent = String.Join(" ", ep.TextContent.Split(' ').Reverse().Take(2).Reverse())
                        );
                    }

                    return episodes;
                }
            }
        }
    }
}
