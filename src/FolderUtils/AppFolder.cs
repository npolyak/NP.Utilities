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
