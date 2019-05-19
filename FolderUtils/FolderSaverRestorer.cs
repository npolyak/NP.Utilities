using System.IO;

namespace NP.Utilities.FolderUtils
{
    public class FolderSaverRestorer : AppFolderUtils
    {
        public FolderSaverRestorer(string folderPath) :
            base(folderPath)
        {

        }

        public string RestoreStr(string locator, string dirName = null)
        {
            if (dirName != null)
            {
                locator = dirName + "\\" + locator;
            }

            string compositionsFilePath =
                GetFilePath(locator);

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
