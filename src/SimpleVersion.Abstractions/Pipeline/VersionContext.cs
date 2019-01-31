using SimpleVersion.Model;

namespace SimpleVersion.Pipeline
{
    public class VersionContext
    {
        public string Path { get; set; }

        public Configuration Configuration { get; set; }
        
        public VersionResult Result { get; set; }        
    }
}
