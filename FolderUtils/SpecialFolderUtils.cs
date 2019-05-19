using System;

namespace NP.Utilities.FolderUtils
{
    public static class SpecialFolderUtils
    {
        public static string AppDataDir { get; } =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    }
}
