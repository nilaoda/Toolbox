using System;
using System.IO;

namespace Ruminoid.Toolbox.Utils
{
    /// <summary>
    /// 提供访问应用存储能力的帮助类型。
    /// </summary>
    public static class StorageHelper
    {
        #region Basic

        /// <summary>
        /// 获取分类文件夹。
        /// </summary>
        /// <param name="sectionName">分类的名称。</param>
        /// <returns>文件夹的完整路径。</returns>
        public static string GetSectionFolderPath(string sectionName)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sectionName + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        /// <summary>
        /// 从分类存储目录中获取文件。
        /// </summary>
        /// <param name="sectionName">分类的名称。</param>
        /// <param name="filename">要获取的文件的文件名。</param>
        /// <returns>文件的完整路径。</returns>
        public static string GetSectionFilePath(string sectionName, string filename) =>
            Path.Combine(GetSectionFolderPath(sectionName), filename);

        #endregion

        #region Temp Section

        public static TempSection CreateTempSection()
        {
            string sectionPath = Path.Combine(GetSectionFolderPath("temp"), Guid.NewGuid().ToString() + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(sectionPath);
            return new TempSection(sectionPath);
        }

        #endregion
    }

    public class TempSection : IDisposable
    {
        internal TempSection(string sectionPath) => SectionPath = sectionPath;

        public readonly string SectionPath;

        ~TempSection() => Dispose();

        public void Dispose() => Directory.Delete(SectionPath, true);
    }
}
