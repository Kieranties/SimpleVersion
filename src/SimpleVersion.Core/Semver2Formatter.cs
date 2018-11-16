namespace SimpleVersion
{
    public class Semver2Formatter : IResultFormatter
    {
        public IVersionResult Format(int height, VersionInfo info) 
            => new Semver2Result(info, height);
    }
}
