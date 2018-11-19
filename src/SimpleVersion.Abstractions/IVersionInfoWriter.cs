namespace SimpleVersion
{
    public interface IVersionInfoWriter
    {
        string ToText(VersionInfo info);

        void ToFile(VersionInfo info, string path);
    }
}
