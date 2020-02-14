namespace NP.Utilities.FolderUtils
{
    public class AppFolderUtils
    {
        public string PathWithRespectToAppDir { get; }

        public string FullBasePath { get; }

        public string Extension { get; }

        public AppFolderUtils(string pathWithRespectToAppDir, string extension = "xml")
        {
            PathWithRespectToAppDir = pathWithRespectToAppDir;

            Extension = extension;

            FullBasePath = SpecialFolderUtils.AppDataDir.AddPath(PathWithRespectToAppDir);

            FullBasePath.CreateDirIfDoesNotExist();
        }

        public string GetFilePath(string fileName)
        {
            return FullBasePath.AddPath(fileName) + (Extension.IsNullOrEmpty() ? "" : "." + Extension);
        }
    }
}
