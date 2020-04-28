// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using SimpleVersion.Configuration;

namespace SimpleVersion.Comparers
{
    /// <summary>
    /// Compares <see cref="VersionConfiguration"/> version and labels.
    /// </summary>
    public class VersionConfigurationLabelComparer : IEqualityComparer<VersionConfiguration>
    {
        /// <summary>
        /// Compares two <see cref="VersionConfiguration"/> for version and label equivalence.
        /// </summary>
        /// <param name="x">The first <see cref="VersionConfiguration"/> to compare.</param>
        /// <param name="y">The second <see cref="VersionConfiguration"/> to compare.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public bool Equals(VersionConfiguration x, VersionConfiguration y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.Version == y.Version)
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

        /// <summary>
        /// Generates a hashcode for the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="VersionConfiguration"/> to generate a hashcode for.</param>
        /// <returns>The generated hashcode.</returns>
        public int GetHashCode(VersionConfiguration configuration)
        {
            Assert.ArgumentNotNull(configuration, nameof(configuration));

            return (configuration.Version.GetHashCode(StringComparison.OrdinalIgnoreCase) * 17) + configuration.Label.GetHashCode();
        }
    }
}
