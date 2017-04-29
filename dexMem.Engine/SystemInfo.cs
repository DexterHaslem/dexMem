/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/
namespace DexMem.Engine
{
    internal class SystemInfo
    {
        public static readonly long MinApplicationAddress;

        public static readonly long MaxApplicationAddress;
        //private static uint _pageSize;

        static SystemInfo()
        {
            NativeMethods.GetSystemInfo(out NativeMethods.SYSTEM_INFO systemInfo);
            MinApplicationAddress = systemInfo.minimumApplicationAddress.ToInt64();
            MaxApplicationAddress = systemInfo.maximumApplicationAddress.ToInt64();
            //_pageSize = systemInfo.pageSize;
        }
    }
}
