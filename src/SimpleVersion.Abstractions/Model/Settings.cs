// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;

namespace SimpleVersion.Model
{
#pragma warning disable CA2227 // Collection properties should be read only
    /// <summary>
    /// Encapsulates settings loaded from '.simpleversion.json'.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the base version string.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the offset to apply to the generated height.
        /// </summary>
        public int OffSet { get; set; } = 0;

        /// <summary>
        /// Gets or sets the label parts to use in the generated version.
        /// </summary>
        public List<string> Label { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the Metadata parts to use in the generated version.
        /// </summary>
        public List<string> Metadata { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the information on branches.
        /// See <see cref="BranchInfo"/> for further details.
        /// </summary>
        public BranchInfo Branches { get; set; } = new BranchInfo();
    }
#pragma warning restore CA2227 // Collection properties should be read only
}
