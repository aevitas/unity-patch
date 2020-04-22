using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace Patcher
{
    using System.Runtime.InteropServices;

    internal static class Program
    {
        internal static void Main(string[] args)
        {
            var themeName = string.Empty;
            var help = false;
            var fileLocation = string.Empty;
            var os = OperatingSystem.Unknown;
            var version = string.Empty;
            var force = false;

            var optionSet = new OptionSet
            {
                {"theme=|t=", "The theme to be applied to the Unity.", v => themeName = v},
                {
                    "exe=|e=", "The location of the Unity Editor executable.",
                    v => fileLocation = v
                },
                {
                    "mac", "Specifies if the specified binary is the MacOS version of Unity3D.",
                    v =>
                    {
                        if (v != null) os = OperatingSystem.MacOS;
                    }
                },
                {
                    "linux", "Specifies if the specified binary is the Linux version of Unity3D.",
                    v =>
                    {
                        if (v != null) os = OperatingSystem.Linux;
                    }
                },
                {
                    "windows", "Specifies if the specified binary is the Windows version of Unity3D.",
                    v =>
                    {
                        if (v != null) os = OperatingSystem.Windows;
                    }
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

            if (help)
            {
                Console.WriteLine("Usage: patcher.exe");
                optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }

            if (os == OperatingSystem.Unknown)
            {
                os = DetectOperatingSystem();
                if (os == OperatingSystem.Unknown)
                {
                    Console.WriteLine(
                        "Failed to detect OS and non was specified - please specify a valid operating system. See patcher -h for available options.");
                    return;
                }
            }

            var patches = Patches.GetPatches(os);

            var patch = patches.FirstOrDefault(p => p.Version.Equals(version, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(fileLocation))
            {
                // https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html
                var unityInstallations = UnityInstallation.GetUnityInstallations(os);

                Console.WriteLine("Please choose the editor which should get patched:");
                UnityInstallation selectedInstallation = ConsoleUtility.ShowSelectionMenu(unityInstallations.ToList(),
                    installation =>
                    {
                        var supported = installation.IsSupported(patches) ? "Supported" : "Unsupported";
                        return $"{installation.Version}\t({supported})";
                    });

                fileLocation = selectedInstallation.ExecutablePath();
                version = selectedInstallation.Version;

                patch ??= selectedInstallation.GetPatch(patches);

                if (patch == null)
                {
                    Console.WriteLine("Couldn't find Patch for specified Unity Installation, please choose one:");
                    patch = ConsoleUtility.ShowSelectionMenu(patches, patchInfo => patchInfo.Version);
                }
            }

            if (patch == null)
            {
                patch = patches.First();
                Console.WriteLine(string.IsNullOrWhiteSpace(version)
                    ? $"Version not explicitly specified -- defaulting to version {patch.Version} for {os}"
                    : $"Could not find patch details for {os} Unity version {version} -- defaulting to version {patch.Version}.");
            }
            else
            {
                Console.WriteLine($"Applying Patch for {patch.Version}");
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

                CreateBackup(fileInfo, ms);
                PatchExecutable(ms, fs, patch, themeName, force);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "Could not open the Unity executable - are you running the patcher as an administrator?");
            }
        }

        private static void CreateBackup(FileSystemInfo fileInfo, MemoryStream ms)
        {
            Console.WriteLine("Creating backup...");

            var backupFileInfo = new FileInfo(fileInfo.FullName + ".bak");

            if (backupFileInfo.Exists)
                backupFileInfo.Delete();

            using var backupWriteStream = backupFileInfo.OpenWrite();
            
                backupWriteStream.Write(ms.ToArray(), 0, (int)ms.Length);
            

            if (backupFileInfo.Exists)
                Console.WriteLine($"Backup '{backupFileInfo.Name}' created.");
        }

        private static void PatchExecutable(MemoryStream ms, FileStream fs, PatchInfo patch, string themeName,
            bool force)
        {
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
                Console.WriteLine(
                    $"Warning: Found more than one occurrence of the theme offset in the specified executable. There is a chance that patching it leads to undefined behaviour. It could also just work fine.{Environment.NewLine}{Environment.NewLine}");
                Console.WriteLine(
                    "Run the patcher with the --force option if you want to patch regardless of this warning.");

                if (!force)
                    return;
            }

            var themeBytes = themeName == "dark" ? patch.DarkPattern : patch.LightPattern;

            Console.WriteLine($"Patching to {themeName}...");

            foreach (var offset in offsets)
            
                for (var i = 0; i < themeBytes.Length; i++)
                {
                    fs.Position = offset + i;
                    fs.WriteByte(themeBytes[i]);
                }
            

            Console.WriteLine("Unity was successfully patched. Enjoy!");
        }

        private static IEnumerable<int> FindPattern(byte[] needle, byte[] haystack)
        {
            return new BinarySearcher(needle).Search(haystack).ToArray();
        }

        private static OperatingSystem DetectOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OperatingSystem.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OperatingSystem.MacOS;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OperatingSystem.Linux;
            }

            return OperatingSystem.Unknown;
        }
    }
}
