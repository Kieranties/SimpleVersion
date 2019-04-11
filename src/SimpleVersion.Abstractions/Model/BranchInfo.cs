// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;

namespace SimpleVersion.Model
{
    /// <summary>
    /// Encapsulates configuration for branches.
    /// </summary>
    public class BranchInfo
    {
        /// <summary>
        /// Gets the list of branches that may produce release versions.
        /// </summary>
        public List<string> Release { get; } = new List<string>();

        /// <summary>
        /// Gets the configuration for branches which override the defaults.
        /// See <see cref="BranchConfiguration"/> for further details.
        /// </summary>
        public List<BranchConfiguration> Overrides { get; } = new List<BranchConfiguration>();
    }
}
