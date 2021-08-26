// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System.IO;

namespace NP.Utilities.FolderUtils
{
    public class ItemToFolderSaverRestorer : AppFolderUtils
    {
        public ItemToFolderSaverRestorer(string folderPath, string extension = "xml") :
            base(folderPath, extension)
        {
        }

        public string GetFullPath(string locator, string dirName = null)
        {
            if (dirName != null)
            {
                locator = dirName + "\\" + locator;
            }

            locator = GetFilePath(locator);
            
            return locator;
        }

        public string RestoreStr(string locator, string dirName = null)
        {
            string compositionsFilePath = GetFullPath(locator, dirName);

            if (!File.Exists(compositionsFilePath))
                return null;

            using (StreamReader reader = new StreamReader(compositionsFilePath))
            {
                return reader.ReadToEnd();
            }
        }

        public byte[] RestoreBytes(string locator, string dirName = null)
        {
            string compositionsFilePath = GetFullPath(locator, dirName);

            if (!File.Exists(compositionsFilePath))
                return null;

            using FileStream fileStream = new FileStream(compositionsFilePath, FileMode.Open, FileAccess.Read);

            byte[] bytes = new byte[fileStream.Length];

            fileStream.Read(bytes, 0, (int) fileStream.Length);

            return bytes;
        }

        private string CreateDirIfNeededGetFilePath(string locator, string dirName = null)
        {
            if (dirName != null)
            {
                string fullDirName = FullBasePath.AddPath(dirName);
                if (!Directory.Exists(fullDirName))
                {
                    Directory.CreateDirectory(fullDirName);
                }

                locator = dirName + "\\" + locator;
            }

            string compositionsFilePath =
                GetFilePath(locator);

            return compositionsFilePath;
        }

        public string SaveStr(string locator, string text, string dirName = null)
        {
            string compositionsFilePath =
                CreateDirIfNeededGetFilePath(locator, dirName);

            using (StreamWriter writer = new StreamWriter(compositionsFilePath))
            {
                writer.Write(text);

                writer.Flush();
            }

            return compositionsFilePath;
        }

        public string SaveBytes(string locator, byte[] bytes, string dirName = null)
        {
            string compositionsFilePath =
                CreateDirIfNeededGetFilePath(locator, dirName);

            using var fileStream = new FileStream(compositionsFilePath, FileMode.OpenOrCreate, FileAccess.Write);

            fileStream.Write(bytes, 0, bytes.Length);

            fileStream.Flush();

            return compositionsFilePath;
        }

        public void DeleteItem(string locator, string dirName = null)
        {
            locator = GetFullPath(locator, dirName);

            if (File.Exists(locator))
            {
                File.Delete(locator);
            }
        }
    }
}
