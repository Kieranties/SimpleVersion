using System;
using System.Collections.Generic;

namespace SimpleVersion
{
    public class BranchInfo
    {
        public List<string> Release { get; } = new List<string>();

        public bool AddShortShaToNonRelease = true;
    }
}
