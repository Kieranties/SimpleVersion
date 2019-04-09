using SimpleVersion.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Comparers
{
    public class ConfigurationVersionLabelComparer : IEqualityComparer<Configuration>
    {        
        public bool Equals(Configuration x, Configuration y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null && y != null)
                return false;

            if (y == null && x != null)
                return false;

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

        public int GetHashCode(Configuration configuration)
        {
            if(configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            if (configuration.Version is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException("Version");

            if (configuration.Label is null)
                throw new ArgumentNullException("Label");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            return configuration.Version.GetHashCode() * 17 + configuration.Label.GetHashCode();
        }
    }
}
