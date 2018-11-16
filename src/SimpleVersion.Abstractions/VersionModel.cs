using System;
using System.Collections.Generic;

namespace SimpleVersion
{
    public class VersionModel : IEquatable<VersionModel>
    {
        public string Version { get; set; } = string.Empty;

        public List<string> Label { get; } = new List<string>();

        public List<string> MetaData { get; } = new List<string>();

        public bool Equals(VersionModel other)
        {
            if (other == null) return false;

            return Version.Equals(other.Version)
                && Label.Equals(other.Label);
        }
    }
}
