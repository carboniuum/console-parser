using ConsoleParser.Core;
using System;
using System.Threading.Tasks;

namespace ConsoleParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new UserInteraction().StartAsync();

            Console.ReadKey();
        }
    }

}
