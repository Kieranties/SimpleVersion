namespace SimpleVersion
{
    public class Semver2Formatter : IResultFormatter
    {
        public IVersionResult Format(int height, VersionModel model) => new Semver2Result(model, height);
    }
}
