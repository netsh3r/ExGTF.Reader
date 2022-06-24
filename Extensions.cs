using System;
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
    }
}