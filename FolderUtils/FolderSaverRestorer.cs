using System.IO;

namespace NP.Utilities.FolderUtils
{
    public class FolderSaverRestorer : AppFolderUtils
    {
        public FolderSaverRestorer(string folderPath) :
            base(folderPath)
        {

        }

        private string GetFullPath(string locator, string dirName = null)
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

        public void SaveStr(string locator, string text, string dirName = null)
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

            using (StreamWriter writer = new StreamWriter(compositionsFilePath))
            {
                writer.Write(text);
            }
        }

        public void DeleteItem(string locator, string dirName = null)
        {
            locator = GetFullPath(locator, dirName);

            if (File.Exists(locator))
            {
                File.Delete(locator);
            }
        }

        //public T RestoreAndDeserialize<T>(string locator)
        //{
        //    string restoredObsStr = RestoreStr(locator);
        //    T result = XmlSerializationUtils.Deserialize<T>(restoredObsStr);

        //    return result;
        //}

        //public void SerializeAndSave<T>(T objToSave, string locator)
        //{
        //    string objStr = objToSave.Serialize();

        //    SaveStr(locator, objStr);
        //}
    }
}
