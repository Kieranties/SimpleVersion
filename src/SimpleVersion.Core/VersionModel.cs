using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SimpleVersion.Core
{
    public class VersionModel
    {
        public static VersionModel Load(string path = ".")
        {
            var basePath = Path.GetFullPath(path);
            var fullPath = Path.Combine(basePath, "version.json");
            if (!File.Exists(fullPath))
            {
                //TODO: Fix up exception
                throw new FileNotFoundException("Could not find SimpleVersion configuration", fullPath);
            }

            return JsonConvert.DeserializeObject<VersionModel>(File.ReadAllText(fullPath));
        }

        public string Version { get; set; } = string.Empty;

        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 1;
        public int Patch { get; set; } = 0;
        public int? Revision { get; set; }

        public List<string> Label { get; } = new List<string>();

        public List<string> MetaData { get; } = new List<string>();
    }
}
