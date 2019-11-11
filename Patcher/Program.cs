using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace Patcher
{
    class Program
    {
        // When the editor is currently using the light theme, you'll find the light pattern. If it's set to the dark pattern, you'll find the dark one.
        // Patterns are for the Windows build of Unity, version 2019.2.3f1.
        private static byte[] _lightPattern = { 0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49 };
        private static byte[] _darkPattern = { 0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49 };

        internal static void Main(string[] args)
        {
            var themeName = string.Empty;
            var help = false;
            var fileLocation = @"C:\Program Files\Unity\Editor\Unity.exe";
            var mac = false;
            var linux = false;

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
                {"help|h", v => help = v != null}
            };

            string error = null;
            try
            {
                List<string> leftover = optionSet.Parse(args);
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
                Console.WriteLine($"Please specify the path to the unity executable, or leave it blank to use the default.");
                return;
            }

            if (help)
            {
                Console.WriteLine("Usage: patcher.exe");
                optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }

            if (mac)
            {
                // Unity 2019.1.0f2 for MacOS.
                _lightPattern = new byte[] { 0x74, 0x03, 0x41, 0x8b, 0x06, 0x48 };
                _darkPattern = new byte[] { 0x75, 0x03, 0x41, 0x8b, 0x06, 0x48 };
            }

            if (linux)
            {
                // Unity 2019.2.3f for Linux.
                _lightPattern = new byte[] { 0x74, 0x02, 0x8b, 0x03, 0x48, 0x83 };
                _darkPattern = new byte[] { 0x75, 0x02, 0x8b, 0x03, 0x48, 0x83 };
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

                using (var fs = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);

                    Console.WriteLine("Creating backup..");

                    var backupFileInfo = new FileInfo(fileInfo.FullName + ".bak");

                    if (backupFileInfo.Exists)
                        backupFileInfo.Delete();

                    using (var backupWriteStream = backupFileInfo.OpenWrite())
                        backupWriteStream.Write(ms.ToArray(), 0, (int)ms.Length);

                    if (backupFileInfo.Exists)
                        Console.WriteLine("Backup file created.. looks awake.");

                    Console.WriteLine("Searching for theme offset..");

                    var buffer = ms.ToArray();
                    var lightOffsets = FindPattern(_lightPattern, buffer).ToList();
                    var darkOffsets = FindPattern(_darkPattern, buffer).ToList();
                    lightOffsets.AddRange(darkOffsets);

                    var found = lightOffsets.Any();

                    if (!found)
                    {
                        Console.WriteLine("Could not find the theme offset in the specified executable.");
                        return;
                    }

                    byte themeByte = themeName == "dark" ? (byte)0x74 : (byte)0x75;

                    foreach (var offset in lightOffsets)
                    {
                        Console.WriteLine($"Patching theme to {themeName}..");
                        fs.Position = offset;
                        fs.WriteByte(themeByte);
                    }

                    Console.WriteLine("Unity was successfully patched. Enjoy!");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Could not open the Unity executable - are you running the patcher as an administrator?");
            }
        }

        private static IEnumerable<int> FindPattern(byte[] needle, byte[] haystack)
        {
            // This is very slow, especially on 75MB size binaries. But it's also 3 lines of code, and I hate code so less code is better.
            for (int i = 0; i < haystack.Length; i++)
            {
                if (haystack.Skip(i).Take(needle.Length).SequenceEqual(needle))
                    yield return i;
            }
        }
    }
}
