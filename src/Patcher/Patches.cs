using System;
using System.Collections.Generic;
using System.Linq;

namespace Patcher
{
    public static class Patches
    {
        private static readonly List<PatchInfo> WindowsPatches = new List<PatchInfo>
        {
            new PatchInfo
            {
                Version = "2018.2",
                LightPattern = new byte[] {0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30},
                DarkPattern = new byte[] {0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30}
            },
            new PatchInfo
            {
                Version = "2018.3",
                LightPattern = new byte[] {0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30},
                DarkPattern = new byte[] {0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30}
            },
            new PatchInfo
            {
                Version = "2018.4",
                LightPattern = new byte[] {0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30},
                DarkPattern = new byte[] {0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30}
            },
            new PatchInfo
            {
                Version = "2019.2",
                LightPattern = new byte[] {0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49},
                DarkPattern = new byte[] {0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49}
            },
            new PatchInfo
            {
                Version = "2019.3",
                LightPattern = new byte[] {0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49},
                DarkPattern = new byte[] {0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49}
            },
            new PatchInfo
            {
                Version = "2020.1",
                LightPattern = new byte[] {0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49},
                DarkPattern = new byte[] {0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49}
            }
        };
        
        private static readonly List<PatchInfo> MacPatches = new List<PatchInfo>
        {
            new PatchInfo
            {
                Version = "2018.4",
                DarkPattern = new byte[] {0x75, 0x03, 0x41, 0x8B, 0x06, 0x4C, 0x3B},
                LightPattern = new byte[] {0x74, 0x03, 0x41, 0x8B, 0x06, 0x4C, 0x3B}
            },
            new PatchInfo
            {
                Version = "2019.1",
                DarkPattern = new byte[] {0x75, 0x03, 0x41, 0x8b, 0x06, 0x48},
                LightPattern = new byte[] {0x74, 0x03, 0x41, 0x8b, 0x06, 0x48}
            },
            new PatchInfo
            {
                Version = "2019.2",
                DarkPattern = new byte[] {0x75, 0x04, 0x8b, 0x03, 0xeb, 0x02},
                LightPattern = new byte[] {0x74, 0x04, 0x8b, 0x03, 0xeb, 0x02}
            },
            new PatchInfo
            {
                Version = "2019.3",
                DarkPattern = new byte[] {0x85, 0xD5, 0x00, 0x00, 0x00, 0x8B, 0x03},
                LightPattern = new byte[] {0x84, 0xD5, 0x00, 0x00, 0x00, 0x8B, 0x03}
            },
            new PatchInfo
            {
                Version = "2019.4",
                DarkPattern = new byte[] {0x85, 0xD5, 0x00, 0x00, 0x00, 0x8B, 0x03},
                LightPattern = new byte[] {0x84, 0xD5, 0x00, 0x00, 0x00, 0x8B, 0x03}
            },
            new PatchInfo
            {
                Version = "2020.1",
                DarkPattern = new byte[] {0x75, 0x5E, 0x8B, 0x03, 0xEB},
                LightPattern = new byte[] {0x74, 0x5E, 0x8B, 0x03, 0xEB}
            },
            new PatchInfo
            {
                Version = "2020.2",
                DarkPattern = new byte[] {0x75, 0x5E, 0x8B, 0x03, 0xEB},
                LightPattern = new byte[] {0x74, 0x5E, 0x8B, 0x03, 0xEB}
            }
        };
        
        private static readonly List<PatchInfo> LinuxPatches = new List<PatchInfo>
        {
            new PatchInfo
            {
                Version = "2018.4",
                DarkPattern = new byte[] {0x75, 0x03, 0x41, 0x8b, 0x06, 0x48},
                LightPattern = new byte[] {0x74, 0x03, 0x41, 0x8b, 0x06, 0x48}
            },
            new PatchInfo
            {
                Version = "2019.1",
                DarkPattern = new byte[] {0x75, 0x04, 0x41, 0x8b, 0x45, 0x00, 0x43, 0x83},
                LightPattern = new byte[] {0x74, 0x04, 0x41, 0x8b, 0x45, 0x00, 0x43, 0x83}
            },
            new PatchInfo
            {
                Version = "2019.2",
                DarkPattern = new byte[] {0x75, 0x02, 0x8b, 0x03, 0x48, 0x83},
                LightPattern = new byte[] {0x74, 0x02, 0x8b, 0x03, 0x48, 0x83}
            },
            new PatchInfo
            {
                Version = "2019.3",
                DarkPattern = new byte[] {0x75, 0x06, 0x41, 0x8b, 0x04, 0x24, 0xeb, 0x02},
                LightPattern = new byte[] {0x74, 0x06, 0x41, 0x8b, 0x04, 0x24, 0xeb, 0x02}
            },
            new PatchInfo
            {
                Version = "2019.4",
                DarkPattern = new byte[] {0x75, 0x06, 0x41, 0x8b, 0x04, 0x24, 0xeb, 0x02},
                LightPattern = new byte[] {0x74, 0x06, 0x41, 0x8b, 0x04, 0x24, 0xeb, 0x02}
            },
            new PatchInfo
            {
                Version = "2020.1",
                DarkPattern = new byte[] {0x75, 0x05, 0x41, 0x8b, 0x07, 0xeb, 0x02},
                LightPattern = new byte[] {0x74, 0x05, 0x41, 0x8b, 0x07, 0xeb, 0x02}
            },
            new PatchInfo
            {
                Version = "2020.2",
                DarkPattern = new byte[] {0x75, 0x5e, 0x8b, 0x03, 0xeb, 0x5c, 0x48},
                LightPattern = new byte[] {0x74, 0x5e, 0x8b, 0x03, 0xeb, 0x5c, 0x48}
            }
        };

        public static List<PatchInfo> GetPatches(OperatingSystem os)
        {
            var patches = os switch
            {
                OperatingSystem.Windows => WindowsPatches,
                OperatingSystem.MacOS => MacPatches,
                OperatingSystem.Linux => LinuxPatches,
                _ => throw new ArgumentOutOfRangeException(nameof(os))
            };

            return patches.OrderByDescending(info => info.Version).ToList();
        }
    }
}
