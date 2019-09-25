// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Comparers
{
    /// <summary>
    /// Compares <see cref="Settings"/> version and labels.
    /// </summary>
    public class ConfigurationVersionLabelComparer : IEqualityComparer<Settings>
    {
        /// <summary>
        /// Compares two <see cref="Settings"/> for version and label equivalence.
        /// </summary>
        /// <param name="x">The first <see cref="Settings"/> to compare.</param>
        /// <param name="y">The second <see cref="Settings"/> to compare.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public bool Equals(Settings x, Settings y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x == null && y != null) return false;

            if (y == null && x != null) return false;

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
        /// Generates a hashcode for the given configuration.
        /// </summary>
        /// <param name="configuration">The <see cref="Settings"/> to generate a hashcode for.</param>
        /// <returns>The generated hashcode.</returns>
        public int GetHashCode(Settings configuration)
        {
            return (configuration.Version.GetHashCode() * 17) + configuration.Label.GetHashCode();
        }
    }
}
