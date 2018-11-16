namespace SimpleVersion
{
    public interface IResultFormatter
    {
        IVersionResult Format(int height, VersionInfo info);
    }
}
