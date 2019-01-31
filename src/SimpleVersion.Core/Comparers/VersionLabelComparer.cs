using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Comparers
{
    public class VersionLabelComparer : IEqualityComparer<(string Version, List<string> Label)>
    {        
        public bool Equals(
            (string Version, List<string> Label) x, 
            (string Version, List<string> Label) y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if(x.Version == y.Version)
            {
                if (x.Label != null)
                {
                    return y.Label == null ? false : x.Label.SequenceEqual(y.Label);
                }
                else
                {
                    return y.Label == null;
                }
            }

            return false;
        }

        public int GetHashCode((string Version, List<string> Label) obj)
        {
            if (obj.Version is null)
                throw new ArgumentNullException("Version");

            if (obj.Label is null)
                throw new ArgumentNullException("Label");

            return obj.Version.GetHashCode() ^ obj.Label.GetHashCode();
        }
    }
}
