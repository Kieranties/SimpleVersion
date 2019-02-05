using System.Collections.Generic;

namespace SimpleVersion.Model
{
    public class VersionResult
    {
        public string Version { get; set; } = string.Empty;

        public int Major { get; set; } = 0;

        public int Minor { get; set; } = 0;

        public int Patch { get; set; } = 0;

        public int Revision { get; set; } = 0;

        public int Height { get; set; } = 0;

        public string HeightPadded => Height.ToString("D4");

        public string Sha { get; set; } = string.Empty;

        public string BranchName { get; set; } = string.Empty;

        public IDictionary<string, string> Formats { get; } = new Dictionary<string, string>();
    }
}
