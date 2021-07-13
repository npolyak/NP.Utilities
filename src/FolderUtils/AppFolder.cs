namespace NP.Utilities.FolderUtils
{
    public class AppFolderUtils
    {
        public string PathWithRespectToAppDir { get; }

        public string FullBasePath { get; }

        public string Extension { get; }

        public AppFolderUtils
        (
            string pathWithRespectToAppDir, 
            string extension = "xml",
            bool isPerUser = false)
        {
            PathWithRespectToAppDir = pathWithRespectToAppDir;

            Extension = extension;

            FullBasePath = 
                isPerUser ? 
                    SpecialFolderUtils.UserAppDataDir.AddPath(PathWithRespectToAppDir)
                    :
                    SpecialFolderUtils.AppDataDir.AddPath(PathWithRespectToAppDir);

            FullBasePath.CreateDirIfDoesNotExist();
        }

        public string GetFilePath(string fileName)
        {
            return FullBasePath.AddPath(fileName) + (Extension.IsNullOrEmpty() ? "" : "." + Extension);
        }
    }
}
