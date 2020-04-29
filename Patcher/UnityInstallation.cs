using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Patcher
{
    /// <summary>
    /// Represents an Unity Installation on the Disk. 
    /// </summary>
    public class UnityInstallation
    {
        private readonly string _installationLocation;

        private readonly OperatingSystem _operatingSystem;

        /// <summary>
        /// Creates a new Unity installation
        /// </summary>
        /// <param name="installationLocation">The Path to this specific installation on the File System</param>
        /// <param name="operatingSystem"></param>
        public UnityInstallation(string installationLocation, OperatingSystem operatingSystem)
        {
            _installationLocation = installationLocation ?? throw new ArgumentNullException(nameof(installationLocation));
            _operatingSystem = operatingSystem;
        }

        /// <summary>
        /// The Version of the Unity Installation
        /// </summary>
        public string Version => Path.GetFileName(_installationLocation);

        /// <summary>
        /// Gets the Path to the Unity executable on the disk.
        /// </summary>
        /// <returns></returns>
        public string ExecutablePath() => Path.Combine(_installationLocation, _operatingSystem switch
        {
            OperatingSystem.Windows => @"Editor\Unity.exe",
            OperatingSystem.MacOS => "Unity.app/Contents/MacOS/Unity",
            OperatingSystem.Linux => "Unity",
            _ => throw new ArgumentOutOfRangeException(nameof(_operatingSystem))
        });

        public bool IsSupported(IEnumerable<PatchInfo> patches)
        {
            return GetPatch(patches) != null;
        }

        public PatchInfo GetPatch(IEnumerable<PatchInfo> patches)
        {
            foreach (PatchInfo patch in patches)
            {
                if (Regex.IsMatch(Version, patch.Version))
                {
                    return patch;
                }
            }

            return null;
        }

        public static IEnumerable<UnityInstallation> GetUnityInstallations(OperatingSystem operatingSystem)
        {
            var path = operatingSystem switch
            {
                OperatingSystem.Windows => @"C:\Program Files\Unity\Hub\Editor\",
                OperatingSystem.MacOS => "/Applications/Unity/Hub/Editor",
                OperatingSystem.Linux => "~/Unity/Hub/Editor",
                _ => throw new ArgumentOutOfRangeException(nameof(operatingSystem))
            };

            if (!Directory.Exists(path)) yield break;
            
            var directories = Directory.GetDirectories(path);
            Array.Sort(directories);
            foreach (string directory in directories)
            {
                yield return new UnityInstallation(directory, operatingSystem);
            }
        }
    }
}
