namespace SimpleVersion
{
    public interface IVersionFormat
    {
        void Apply(VersionInfo info, VersionResult result);
    }
}
