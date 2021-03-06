using System;

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static class PathExtension
    {
        public static string GetMidString(
            this string source,
            string before,
            string after)
        {
            var result = string.Empty;
            try
            {
                var startIndex = source.IndexOf(before, StringComparison.Ordinal);

                if (startIndex == -1)
                    return result;

                var substring = source.Substring(startIndex + before.Length);

                var endIndex = substring.IndexOf(after, StringComparison.Ordinal);

                if (endIndex == -1)
                    return result;

                result = substring.Remove(endIndex);
            }
            catch (Exception)
            {
                // Ignore
            }

            return result;
        }

        public static string EscapePathStringForArg(
            this string path) =>
            $"\"{path}\"";

        public static string EscapeForCode(
            this string str) =>
            System.Text.RegularExpressions.Regex.Escape(str);
    }
}
