using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Model
{
    public class Configuration : IEquatable<Configuration>
    {
        public string Version { get; set; } = string.Empty;

        public int OffSet { get; set; } = 0;

        public List<string> Label { get; } = new List<string>();

        public List<string> MetaData { get; } = new List<string>();

        public BranchInfo Branches { get; set; } = new BranchInfo();

        public bool Equals(Configuration other)
        {
            if (other == null) return false;

            return Version.Equals(other.Version)
                && Label.SequenceEqual(other.Label);
        }
    }
}
