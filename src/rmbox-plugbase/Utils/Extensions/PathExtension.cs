using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static partial class PathExtension
    {
        #region Consts

        public static readonly char[] InvalidChars = { '<', '>', '(', ')', '@', '^', '|', ' ' };

        #endregion

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
            string.IsNullOrWhiteSpace(path) ? string.Empty : Path.GetFullPath(path, Environment.CurrentDirectory);
    }
}
