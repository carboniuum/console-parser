using ConsoleParser.Core;
using System;
using System.Threading.Tasks;

namespace ConsoleParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                await new UserInteraction().StartAsync();
            }
            catch
            {
                Console.WriteLine("Ничего не найдено. Убедитесь, что вы ввели название аним правильно. " +
                    "(Например: death note, tokyo ghoul...)" + Environment.NewLine);

                await new UserInteraction().StartAsync();
            }
        }
    }

}
