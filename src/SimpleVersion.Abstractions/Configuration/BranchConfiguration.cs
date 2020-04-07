// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;

namespace SimpleVersion.Configuration
{
    /// <summary>
    /// Encapsulates configuration for branches.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for System.Text.Json")]
    public class BranchConfiguration
    {
        /// <summary>
        /// Gets or sets the list of branches that may produce release versions.
        /// </summary>
        public List<string> Release { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the configuration for branches which override the defaults.
        /// See <see cref="BranchOverrideConfiguration"/> for further details.
        /// </summary>
        public List<BranchOverrideConfiguration> Overrides { get; set; } = new List<BranchOverrideConfiguration>();
    }
}
