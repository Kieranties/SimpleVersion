using System.Collections.Generic;

namespace SimpleVersion.Model
{
    public class BranchConfiguration
    {
        public string Match { get; set; } = string.Empty;

        public List<string> Label { get; } = new List<string>();

        public List<string> MetaData { get; } = new List<string>();
    }
}
