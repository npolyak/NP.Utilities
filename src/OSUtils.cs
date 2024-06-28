// (c) Nick Polyak 2024 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System.Runtime.InteropServices;

namespace NP.Utilities
{
    public static class OSUtils
    {
        public static bool IsWindows { get; }
        public static bool IsMac { get; }
        public static bool IsLinux { get; }

        static OSUtils()
        {
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}
