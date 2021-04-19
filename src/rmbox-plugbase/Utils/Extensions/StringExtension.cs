using System;
using System.IO;
using System.Linq;

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static class StringExtension
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

        public static string[] GetLines(
            this string str) =>
            str
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

        public static string ReadStreamToEnd(
            this Stream stream)
        {
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
