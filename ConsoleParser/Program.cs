using ConsoleParser.Core;
using System;
using System.Threading.Tasks;
using ConsoleParser.Arts;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Main main = new Main();

            Console.WriteLine("Добро пожаловать в приложение по скачыванию аниме!");
            Console.Write($"Введите название аниме, которое хотите скачать{Environment.NewLine}" +
                $"(Рекомендуется ввести роигинальное название аниме латинскими буквами)" +
                $">>> ");

            string userInput = Console.ReadLine();

            var result = await main.GetSearchResult(userInput);

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
            }

     /*       foreach (var item in result)
            {
                Console.WriteLine($"{item.Key} {item.Value}");
            }
*/

            Console.ReadKey();
        }

        
    }
}
