using System;
using System.Collections.Generic;

namespace SimpleVersion.Model
{
    public class BranchInfo
    {
        public List<string> Release { get; } = new List<string>();

        public List<BranchConfiguration> Overrides { get; } = new List<BranchConfiguration>();
    }
}
