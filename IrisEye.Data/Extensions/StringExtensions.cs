using System;
using System.Text.RegularExpressions;
using RestSharp;

namespace IrisEye.Data.Extensions
{
    public static class StringExtensions
    {
        public static string GetBetween(this string content, string startString, string endString=null)
        {
            if (endString == null)
                endString = "\n";
            if (content.Contains(startString) && content.Contains(endString))
            {
                var start=content.IndexOf(startString, 0, StringComparison.Ordinal) + startString.Length;
                var end=content.IndexOf(endString, start, StringComparison.Ordinal);
                return content.Substring(start, end - start);
            }

            if (endString != "\n") return string.Empty;
            {
                var start=content.IndexOf(startString, 0, StringComparison.Ordinal) + startString.Length;
                return content.Substring(start);
            }

        }

        public static int ToInt(this string content)
        {
            int.TryParse(content, out var ret);
            return ret;

        }

        public static string TrimLimit(this string content, int limit)
        {
            if (content.Length > limit)
            {
                return content.Substring(0, limit) + "...";
            }

            return content;

        }

        public static string ClearTestName(this string content)
        {
            content = content.Trim().ToLower()
                .Replace(" a ", " ")
                .Replace(" the ", " ")
                .Replace("  ", " ");
            var rgx = new Regex(@"[^a-z^\s]+");
            var cleared = rgx.Replace(content.Trim(), "");
            cleared = cleared.Replace(" ", "_").Replace("__","_");
            if (cleared[0] == '_')
            {
                cleared = cleared.Substring(1);
            }

            return cleared;
        }
        
        public static string ConvertEnvironment(this string env)
        {
            switch (env.ToLower())
            {
                case "win":
                    return "Win 10";
                case "win7":
                    return "Win 7";
                case "linux":
                    return "Linux";
                case "osx":
                    return "OSX";
                default:
                    return env;
            }
        }
    }
}