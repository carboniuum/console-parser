using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleParser.Core
{
    public class UserInteraction
    {
        public async Task StartAsync()
        {
            Main main = new Main();
            StringBuilder sb = new StringBuilder();

            Console.WriteLine("Добро пожаловать в приложение по скачыванию аниме!");
            Console.Write($"Введите название аниме, которое хотите скачать{Environment.NewLine}" +
                $"(Рекомендуется ввести роигинальное название аниме латинскими буквами){Environment.NewLine}" +
                $">>> ");

            string userInput = Console.ReadLine();

            var result = await main.GetSearchResultAsync(userInput);

            if (result.First().Key.Contains("season"))
            {
                Console.WriteLine("Выберите сезон:");
                var seasons = new List<int>();
                for (int i = 1; i < 20; i++)
                {
                    foreach (var res in result)
                    {
                        if (res.Key.Contains($"season-{i}"))
                        {
                            seasons.Add(i);
                            break;
                        }
                    }
                }
                string seasonOptions = String.Empty;
                foreach (int season in seasons)
                {
                    seasonOptions += $"{season}, ";
                }
                seasonOptions = seasonOptions.Remove(seasonOptions.Length - 2);
                Console.WriteLine(seasonOptions);
                Console.Write(">>> ");
                string seasonChoice = Console.ReadLine();
                result = result.Where(res => res.Key.Contains($"season-{seasonChoice}"))
                                .ToDictionary(k => k.Key, v => v.Value);
            }

            sb.AppendLine("Выберите серию. Введите только номер эпизода.");

            foreach (var item in result.Values)
            {
                sb.AppendLine(item);
            }

            Console.WriteLine(sb.ToString());
            Console.Write(">>> ");
            string episodeChoice = Console.ReadLine();
            string url = result.FirstOrDefault(res => res.Key.Contains($"episode-{episodeChoice}")).Key;
            var list = await main.GetEpisodeAsync(url);
            sb.Clear();

            sb.AppendLine("Выберите качество.");

            foreach (var item in list)
            {
                sb.AppendLine(item.Quality.ToString());
            }

            Console.WriteLine(sb.ToString());
            Console.Write(">>> ");
            string qualityChoice = Console.ReadLine();
            var video = list.FirstOrDefault(v => v.Quality == Int32.Parse(qualityChoice));

            string videoName = $"{userInput} {episodeChoice} серия.mp4".Replace(' ', '_');
            main.DownloadVideo(video, videoName);
        }
    }
}
