using System;
using System.IO;

namespace NP.Utilities.FolderUtils
{
    public static class IOUtils
    {
        public const char DirEndChar = '\\';
        public const char DirAltEndChar = '/';

        public static void CreateDirIfDoesNotExist(this string path)
        {
            if (path == null)
                return;

            string[] pathLinks =
                path.Split(new[] { DirEndChar, DirAltEndChar }, StringSplitOptions.RemoveEmptyEntries);

            string partialPath = string.Empty;

            foreach (string pathLink in pathLinks)
            {
                partialPath += pathLink + DirEndChar;

                if (!Directory.Exists(partialPath))
                {
                    Directory.CreateDirectory(partialPath);
                }
            }
        }

        public static string UnifyFolderSeparator(this string path)
        {
            return path?.Replace(DirAltEndChar, DirEndChar);
        }

        public static string EnforceEndChar(this string path)
        {
            path = path.UnifyFolderSeparator().Trim();

            if (!path.EndsWith("" + DirEndChar))
            {
                path += DirEndChar;
            }

            return path;
        }

        public static string AddPath(this string basePath, string extraPath)
        {
            if (basePath == null)
                return extraPath;

            if (extraPath == null)
                return basePath;

            basePath = basePath.EnforceEndChar();

            return basePath + extraPath;
        }
    }
}
