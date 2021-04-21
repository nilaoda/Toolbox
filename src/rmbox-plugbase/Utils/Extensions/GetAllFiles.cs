using System.Collections.Generic;
using System.IO;

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static partial class PathExtension
    {
        public static List<string> GetAllFiles(
            string directory)
        {
            List<string> result = new();
            if (string.IsNullOrEmpty(directory)) return result;

            GetAllFilesIntl(directory, result);
            return result;
        }

        private static void GetAllFilesIntl(
            string directory,
            List<string> result)
        {
            result.AddRange(Directory.GetFiles(directory));
            foreach (string d in Directory.GetDirectories(directory)) GetAllFilesIntl(d, result);
        }
    }
}
