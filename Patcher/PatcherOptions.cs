namespace Patcher
{
    public class PatcherOptions
    {
        public string ThemeName { get; set; }

        public string FileLocation { get; set; }

        public string Version { get; set; }

        public OperatingSystem? OperatingSystem { get; set; }

        public bool? Force { get; set; }
    }
}
