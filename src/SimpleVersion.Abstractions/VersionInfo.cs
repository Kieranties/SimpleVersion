using System;
using System.Collections.Generic;

namespace SimpleVersion
{
    public class VersionInfo : IEquatable<VersionInfo>
    {
        public string Version { get; set; } = string.Empty;

        public int OffSet { get; set; } = 0;

        public List<string> Label { get; } = new List<string>();

        public List<string> MetaData { get; } = new List<string>();

        public bool Equals(VersionInfo other)
        {
            if (other == null) return false;

            return Version.Equals(other.Version)
                && Label.Equals(other.Label);
        }
    }
}
