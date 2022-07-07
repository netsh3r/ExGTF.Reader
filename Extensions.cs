using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExGTF.Reader
{
    public static class Extensions
    {
        public static void ForEach(this MatchCollection collection, Action<Match, int> action)
        {
            int i = 0;
            foreach (Match val in collection)
            {
                action.Invoke(val, i++);
            }
        }

        public static string Get(this Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value.ToString();
            }

            return string.Empty;
        }

        public static Array GetArray(this Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value) && value is Array array)
            {
                return array;
            }

            return Array.Empty<object>();
        }
    }
}