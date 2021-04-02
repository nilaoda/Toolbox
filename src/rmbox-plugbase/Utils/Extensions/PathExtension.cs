using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static partial class PathExtension
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

        public static string EscapePathStringForArg(
            this string path,
            bool useSingleQuote = false) =>
            useSingleQuote ? $"'{path}'" : $"\"{path}\"";

        public static string EscapeQuote(
            this string str) =>
            str.Replace("\"", "\\\"");

        public static string EscapeForCode(
            this string str) =>
            str.Aggregate(string.Empty, (s, c) => s + c.EscapeForCode());

        public static string EscapeForCode(
            this char c) =>
            "\\" + "u" + ((int)c).ToString("x4");

        public static string GetTargetPath(
            string target) =>
            Path.Combine(
                StorageHelper.GetSectionFolderPath("tools"),
                target + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));

        public static string GetFullPathOrEmpty(
            string path) =>
            string.IsNullOrWhiteSpace(path) ? string.Empty : Path.GetFullPath(path);
    }
}
