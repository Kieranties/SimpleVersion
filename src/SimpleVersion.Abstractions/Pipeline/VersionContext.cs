﻿using SimpleVersion.Model;

namespace SimpleVersion.Pipeline
{
    public class VersionContext
    {
        public string Path { get; set; } = string.Empty;

        public string RepositoryPath { get; set; } = string.Empty;

        public Configuration Configuration { get; set; } = new Configuration();

        public VersionResult Result { get; set; } = new VersionResult();
    }
}