using System;
using System.IO;
using Newtonsoft.Json;

namespace SimpleVersion
{
    public class JsonVersionModelReader : IVersionModelReader
    {
        public VersionModel Load() => Load(Directory.GetCurrentDirectory());

        public VersionModel Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Null or empty value", nameof(path));
            }

            var basePath = Path.GetFullPath(path);
            var fullPath = Path.Combine(basePath, Constants.VersionFileName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Could not find version file", fullPath);
            }

            return Read(File.ReadAllText(fullPath));
        }

        public VersionModel Read(string text) => JsonConvert.DeserializeObject<VersionModel>(text);
    }
}
