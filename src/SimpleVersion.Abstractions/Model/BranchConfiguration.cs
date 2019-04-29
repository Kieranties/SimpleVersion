// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;

namespace SimpleVersion.Model
{
    /// <summary>
    /// Models the configuration for a branch.
    /// </summary>
    public class BranchConfiguration
    {
        /// <summary>
        /// Gets or sets the regular expression string used to
        /// identify matching to a branch to identify branch matches.
        /// </summary>
        public string Match { get; set; } = string.Empty;

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Gets or sets the label parts to use in the generated version.
        /// </summary>
        public List<string> Label { get; set; } = null;

        /// <summary>
        /// Gets or sets the Metadata parts to use in the generated version.
        /// </summary>
        public List<string> Metadata { get; set; } = null;
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
