// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;

namespace SimpleVersion.Model
{
    /// <summary>
    /// Encapsulates information for branches.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for System.Text.Json")]
    public class BranchInfo
    {
        /// <summary>
        /// Gets or sets the list of branches that may produce release versions.
        /// </summary>
        public List<string> Release { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the settings for branches which override the defaults.
        /// See <see cref="BranchSettings"/> for further details.
        /// </summary>
        public List<BranchSettings> Overrides { get; set; } = new List<BranchSettings>();
    }
}
