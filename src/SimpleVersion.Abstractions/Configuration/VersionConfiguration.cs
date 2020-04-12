// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Configuration
{
    /// <summary>
    /// Configuration for the version to be calculated.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for System.Text.Json")]
    public class VersionConfiguration
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
    }
}
