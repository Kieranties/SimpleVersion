using System;

namespace SimpleVersion
{
    public interface IVersionInfoReader
    {
        VersionInfo Load();

        VersionInfo Load(string path);

        VersionInfo Read(string text);

    }
}
