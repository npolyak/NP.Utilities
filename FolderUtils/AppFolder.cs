namespace NP.Utilities.FolderUtils
{
    public class AppFolderUtils
    {
        public string PathWithRespectToAppDir { get; }

        public string FullBasePath { get; }

        public AppFolderUtils(string pathWithRespectToAppDir)
        {
            PathWithRespectToAppDir = pathWithRespectToAppDir;

            FullBasePath = SpecialFolderUtils.AppDataDir.AddPath(PathWithRespectToAppDir);

            FullBasePath.CreateDirIfDoesNotExist();
        }

        public string GetFilePath(string fileName, string extension = "xml")
        {
            return FullBasePath.AddPath(fileName) + "." + extension;
        }
    }
}
