using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleVersion
{
    public interface IVersionModelWriter
    {
        string ToText(VersionModel model);

        void ToFile(VersionModel model, string path);
    }
}
