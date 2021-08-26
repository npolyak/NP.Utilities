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
