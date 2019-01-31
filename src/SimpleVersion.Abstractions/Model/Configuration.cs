﻿using System.Collections.Generic;

namespace SimpleVersion.Model
{
    public class Configuration
    {
        public string Version { get; set; } = string.Empty;

        public int OffSet { get; set; } = 0;

        public List<string> Label { get; } = new List<string>();

        public List<string> MetaData { get; } = new List<string>();

        public BranchInfo Branches { get; set; } = new BranchInfo();
    }
}
