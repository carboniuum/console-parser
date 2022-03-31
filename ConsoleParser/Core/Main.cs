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
using ConsoleParser.Core.Models;

namespace ConsoleParser.Core
{
    public class Main
    {
        public HtmlParser Parser { get; private set; }
        public Uri BaseAddress { get; private set; }

        public Main()
        {
            Parser = new HtmlParser();
            BaseAddress = new Uri(Settings.BaseUrl);
        }

        public async Task<Dictionary<string, string>> GetSearchResultAsync(string query)
        {
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = this.BaseAddress })
                {
                    client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);
                    var body = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("makeme", "yes"),
                        new KeyValuePair<string, string>("ystext", query)
                    });
                    await Settings.SetCookieAsync();
                    cookieContainer.Add(this.BaseAddress, Settings.SiteCookie);
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

        public async Task<List<Video>> GetEpisodeAsync(string url)
        {
            var cookieContainer = new CookieContainer();
            var episodesList = new List<Video>();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = this.BaseAddress })
                {
                    client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);
                    cookieContainer.Add(this.BaseAddress, Settings.SiteCookie);
                    string result = await client.GetStringAsync(url);
                    var document = await Parser.ParseDocumentAsync(result);
                    var episodeData = document.QuerySelectorAll("#my-player source");

                    foreach (var data in episodeData)
                    {
                        episodesList.Add(
                            new Video
                            {
                                Quality = Int32.Parse(data.GetAttribute("res")),
                                Source = data.GetAttribute("src")
                            }
                        ); ;
                    }

                    return episodesList;
                }
            }
        }

        public void DownloadVideo(Video video)
        {
            /* using (WebClient client = new WebClient())
             {
                 client.DownloadProgressChanged += (o, e) =>
                 {
                     Console.WriteLine($"Идёт скачивание: {e.ProgressPercentage}%.");
                 };

                 client.DownloadDataCompleted += (o, e) =>
                 {
                     Console.WriteLine("Скачивание завершено!");
                 };

                 client.DownloadFileAsync(new Uri(video.Source), "video.mp4");
             }*/
            
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", Settings.UserAgent);
                client.Headers.Add(HttpRequestHeader.Cookie, Settings.SiteCookie.ToString());
                client.DownloadFile(video.Source, "video.mp4");
            }
        }
    }
}
