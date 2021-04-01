using System.IO;
using FormatWith;

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static partial class PathExtension
    {
        /// <summary>
        /// 格式化文件路径。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <param name="formatter">格式字符串。</param>
        /// <returns>格式化的文件路径。</returns>
        public static string FormatPath(
            this string path,
            string formatter) =>
            formatter.FormatWith(new
            {
                folder = Path.GetDirectoryName(path) +
                         (Path.GetDirectoryName(path) is null
                             ? ""
                             : (Path.GetDirectoryName(path).EndsWith(Path.DirectorySeparatorChar)
                                 ? ""
                                 : Path.DirectorySeparatorChar)),
                name = Path.GetFileNameWithoutExtension(path),
                fileName = Path.GetFileName(path),
                extension = Path.GetExtension(path)
            });

        public static string Suffix(
            this string path,
            string suffix) =>
            Path.GetDirectoryName(path) +
            (Path.GetDirectoryName(path) is null
                ? ""
                : (Path.GetDirectoryName(path).EndsWith(Path.DirectorySeparatorChar)
                    ? ""
                    : Path.DirectorySeparatorChar)) +
            Path.GetFileNameWithoutExtension(path) +
            suffix +
            Path.GetExtension(path);
    }
}
