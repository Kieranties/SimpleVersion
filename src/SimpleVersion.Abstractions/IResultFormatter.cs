namespace SimpleVersion
{
    public interface IResultFormatter
    {
        IVersionResult Format(int height, VersionModel model);
    }
}
