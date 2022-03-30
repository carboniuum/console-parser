using System.Collections.Generic;

namespace ConsoleParser.Extensions
{
    public static class DictionaryExtension
    {
        public static void TryAdd(this Dictionary<string, string> dict, string key, string value)
        {
            if (!dict.ContainsKey(key) && key.EndsWith(".html"))
            {
                dict.Add(key, value);
            }
        }
    }
}
