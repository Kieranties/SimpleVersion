namespace SimpleVersion
{
    public interface IVersionFormat
    {
        void Apply(VersionResult result, VersionInfo info);
    }
}
