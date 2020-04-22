namespace Patcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    public class UnityInstallation
    {
        private readonly string _installationLocation;

        public UnityInstallation(string installationLocation)
        {
            _installationLocation = installationLocation;
        }

        public string Version => Path.GetFileName(_installationLocation);

        public string ExecutablePath(OperatingSystem os) => Path.Combine(this._installationLocation, os switch
        {
            OperatingSystem.Windows => @"Editor\Unity.exe",
            OperatingSystem.MacOS => "Unity.app/Contents/MacOS/Unity",
            OperatingSystem.Linux => "Unity",
            _ => throw new ArgumentOutOfRangeException(nameof(os))
        });

        public bool IsSupported(IEnumerable<PatchInfo> patches)
        {
            return GetPatch(patches) != null;
        }

        public PatchInfo GetPatch(IEnumerable<PatchInfo> patches)
        {
            foreach (PatchInfo patch in patches)
            {
                if (Regex.IsMatch(this.Version, patch.Version))
                {
                    return patch;
                }
            }

            return null;
        }

        public static UnityInstallation[] GetUnityInstallations(OperatingSystem operatingSystem)
        {
            try
            {
                var path = operatingSystem switch
                {
                    OperatingSystem.Windows => @"C:\Program Files\Unity\Hub\Editor\",
                    OperatingSystem.MacOS => "/Applications/Unity/Hub/Editor",
                    OperatingSystem.Linux => "~/Unity/Hub/Editor",
                    _ => throw new ArgumentOutOfRangeException("OS is not supported")
                };

                if (Directory.Exists(path))
                {
                    var directories = Directory.GetDirectories(path);
                    Array.Sort(directories);
                    var installations = new UnityInstallation[directories.Length];
                    for (var i = 0; i < directories.Length; i++)
                    {
                        installations[i] = new UnityInstallation(directories[i]);
                    }

                    return installations;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "Could not open the Unity executable - are you running the patcher as an administrator?");
                throw;
            }

            return new UnityInstallation[0];
        }
    }
}
