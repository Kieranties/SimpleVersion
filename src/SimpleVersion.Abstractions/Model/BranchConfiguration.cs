using System.Collections.Generic;

namespace SimpleVersion.Model
{
    public class BranchConfiguration
    {
        public string Match { get; set; } = string.Empty;

        public List<string> Label { get; set; } = null;

        public List<string> MetaData { get; set; } = null;
    }
}
