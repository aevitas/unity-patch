using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace Patcher
{
    internal class Program
    {
        private static readonly Dictionary<string, List<PatchInfo>> Patches = new Dictionary<string, List<PatchInfo>>
        {
            {
                "mac", new List<PatchInfo>
                {
                    new PatchInfo
                    {
                        Version = "2019.1.0f2",
                        DarkPattern = new byte[] {0x75, 0x03, 0x41, 0x8b, 0x06, 0x48},
                        LightPattern = new byte[] {0x74, 0x03, 0x41, 0x8b, 0x06, 0x48}
                    },
                    new PatchInfo
                    {
                        Version = "2019.3.0f6",
                        DarkPattern = new byte[] {0x80, 0x3D, 0x1D, 0x96, 0x83, 0x06, 0x00, 0x0F, 0x85, 0xD5, 0x00, 0x00, 0x00, 0x8B},
                        LightPattern = new byte[] {0x80, 0x3D, 0x1D, 0x96, 0x83, 0x06, 0x00, 0x0F, 0x84, 0xD5, 0x00, 0x00, 0x00, 0x8B}
                    }
                }
            },
            {
                "linux", new List<PatchInfo>
                {
                    new PatchInfo
                    {
                        Version = "2019.2.3f",
                        DarkPattern = new byte[] {0x75, 0x02, 0x8b, 0x03, 0x48, 0x83},
                        LightPattern = new byte[] {0x74, 0x02, 0x8b, 0x03, 0x48, 0x83}
                    }
                }
            },
            {
                "windows", new List<PatchInfo>
                {
                    new PatchInfo
                    {
                        Version = "2019.3.2f1",
                        LightPattern = new byte[] {0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49},
                        DarkPattern = new byte[] {0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49}
                    },
                    new PatchInfo
                    {
                        Version = "2019.2.3f1",
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
                }
            }
        };

        internal static void Main(string[] args)
        {
            var themeName = string.Empty;
            var help = false;
            var fileLocation = @"C:\Program Files\Unity\Hub\Editor\2019.3.2f1\Editor\Unity.exe";
            var windows = false;
            var mac = false;
            var linux = false;
            var version = string.Empty;
            var force = false;

            var optionSet = new OptionSet
            {
                {"theme=|t=", "The theme to be applied to the Unity.", v => themeName = v},
                {"exe=|e=", "The location of the Unity Editor executable.", v => fileLocation = v},
                {
                    "mac", "Specifies if the specified binary is the MacOS version of Unity3D.",
                    v => mac = v != null
                },
                {
                    "linux", "Specifies if the specified binary is the Linux version of Unity3D.",
                    v => linux = v != null
                },
                {
                    "windows", "Specifies if the specified binary is the Windows version of Unity3D.",
                    v => windows = v != null
                },
                {
                    "version=|v=", "The version of Unity to patch.", v => version = v
                },
                {
                    "force|f", "Gently applies force.", v => force = v != null
                },
                {"help|h", v => help = v != null}
            };

            string error = null;
            try
            {
                var leftover = optionSet.Parse(args);
                if (leftover.Any())
                    error = "Unknown arguments: " + string.Join(" ", leftover);
            }
            catch (OptionException ex)
            {
                error = ex.Message;
            }

            if (error != null)
            {
                Console.WriteLine($"Unity Patcher: {error}");
                Console.WriteLine("Use --help for a list of supported options.");
                return;
            }

            if (string.IsNullOrEmpty(themeName))
                themeName = "dark";

            if (!themeName.Equals("dark", StringComparison.OrdinalIgnoreCase) &&
                !themeName.Equals("light", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Theme \"{themeName}\" is not supported - please specify either dark or light.");
                return;
            }

            if (string.IsNullOrEmpty(fileLocation))
            {
                Console.WriteLine(
                    "Please specify the path to the Unity executable, or leave it blank to use the default.");
                return;
            }

            if (help)
            {
                Console.WriteLine("Usage: patcher.exe");
                optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }

            string os = null;

            if (mac)
                os = "mac";

            if (linux)
                os = "linux";

            if (windows)
                os = "windows";

            if (string.IsNullOrWhiteSpace(os))
            {
                Console.WriteLine($"No OS was specified - please specify a valid operating system when running the patcher. See patcher -h for available options.");
                return;
            }

            var patch = Patches[os]
                .FirstOrDefault(p => p.Version.Equals(version, StringComparison.OrdinalIgnoreCase));

            if (patch == null)
            {
                patch = Patches[os].First();
                Console.WriteLine(string.IsNullOrWhiteSpace(version)
                    ? $"Version not explicitly specified -- defaulting to version {patch.Version} for {os}"
                    : $"Could not find patch details for {os} Unity version {version} -- defaulting to version {patch.Version}.");
            }

            Console.WriteLine($"Opening Unity executable from {fileLocation}...");

            try
            {
                var fileInfo = new FileInfo(fileLocation);

                if (!fileInfo.Exists)
                {
                    Console.WriteLine($"Could not find the specified file: {fileInfo.FullName}");
                    return;
                }

                if (fileInfo.IsReadOnly)
                {
                    Console.WriteLine("Can not patch the specified file - it is marked as read only!");
                    return;
                }

                using var fs = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
                using var ms = new MemoryStream();

                fs.CopyTo(ms);

                Console.WriteLine("Creating backup...");

                var backupFileInfo = new FileInfo(fileInfo.FullName + ".bak");

                if (backupFileInfo.Exists)
                    backupFileInfo.Delete();

                using (var backupWriteStream = backupFileInfo.OpenWrite())
                {
                    backupWriteStream.Write(ms.ToArray(), 0, (int)ms.Length);
                }

                if (backupFileInfo.Exists)
                    Console.WriteLine($"Backup '{backupFileInfo.Name}' created.");

                Console.WriteLine("Searching for theme offset...");

                var buffer = ms.ToArray();

                var lightOffsets = FindPattern(patch.LightPattern, buffer).ToArray();
                var darkOffsets = FindPattern(patch.DarkPattern, buffer).ToArray();
                var offsets = new HashSet<int>(lightOffsets);
                offsets.UnionWith(darkOffsets);

                var found = offsets.Any();
                if (!found)
                {
                    Console.WriteLine("Error: Could not find the theme offset in the specified executable!");
                    return;
                }
                var foundMultipleOffsets = offsets.Count > 1;
                if (foundMultipleOffsets)
                {
                    Console.WriteLine($"Warning: Found more than one occurrence of the theme offset in the specified executable. There is a chance that patching it leads to undefined behaviour. It could also just work fine.{Environment.NewLine}{Environment.NewLine}");
                    Console.WriteLine("Run the patcher with the --force option if you want to patch regardless of this warning.");

                    if (!force)
                        return;
                }
                var themeBytes = themeName == "dark" ? patch.DarkPattern : patch.LightPattern;

                Console.WriteLine($"Patching to {themeName}...");

                foreach (var offset in offsets)
                {
                    for (int i = 0; i < themeBytes.Length; i++)
                    {
                        fs.Position = offset + i;
                        fs.WriteByte(themeBytes[i]);
                    }
                }

                Console.WriteLine("Unity was successfully patched. Enjoy!");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "Could not open the Unity executable - are you running the patcher as an administrator?");
            }
        }

        private static IEnumerable<int> FindPattern(byte[] needle, byte[] haystack)
        {
            // This is very slow, especially on 75MB size binaries. But it's also 3 lines of code, and I hate code so less code is better.
            for (var i = 0; i < haystack.Length; i++)
                if (haystack.Skip(i).Take(needle.Length).SequenceEqual(needle))
                    yield return i;
        }

        private class PatchInfo
        {
            public string Version { get; set; }

            public byte[] DarkPattern { get; set; }

            public byte[] LightPattern { get; set; }
        }
    }
}