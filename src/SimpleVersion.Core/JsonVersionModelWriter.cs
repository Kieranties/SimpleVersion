using Newtonsoft.Json;
using System.IO;

namespace SimpleVersion
{
    public class JsonVersionModelWriter : IVersionModelWriter
    {
        public void ToFile(VersionModel model, string path) => File.WriteAllText(path, ToText(model));

        public string ToText(VersionModel model) => JsonConvert.SerializeObject(model, Formatting.Indented);
    }
}
