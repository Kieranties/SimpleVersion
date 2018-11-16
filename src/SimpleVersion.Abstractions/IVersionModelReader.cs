using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion
{
    public interface IVersionModelReader
    {
        VersionModel Load();

        VersionModel Load(string path);

        VersionModel Read(string text);

    }
}
