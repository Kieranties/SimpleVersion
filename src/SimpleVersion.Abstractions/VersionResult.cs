using System.Collections.Generic;

namespace SimpleVersion
{
    public class VersionResult
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public int Revision { get; set; }

        public int Height { get; set; }

        public string HeightPadded => Height.ToString("D4");

        public string Sha { get; set; }

        public string BranchName { get; set; }

        public IDictionary<string, string> Formats { get; } = new Dictionary<string, string>();
    }
}
