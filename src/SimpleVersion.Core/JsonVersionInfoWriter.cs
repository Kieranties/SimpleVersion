using Newtonsoft.Json;
using System;
using System.IO;

namespace SimpleVersion
{
    public class JsonVersionInfoWriter : IVersionInfoWriter
    {
        public void ToFile(VersionInfo info, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Null or empty value", nameof(path));

            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
                Directory.CreateDirectory(dir.FullName);

            var fullPath = Path.Combine(dir.FullName, Constants.VersionFileName);
            File.WriteAllText(fullPath, ToText(info));
        }

        public string ToText(VersionInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return JsonConvert.SerializeObject(info, Formatting.Indented);
        }
    }
}
