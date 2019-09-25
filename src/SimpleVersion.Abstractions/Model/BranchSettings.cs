// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;

namespace SimpleVersion.Model
{
    /// <summary>
    /// Models the settings for a branch.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for System.Text.Json")]
    public class BranchSettings
    {
        /// <summary>
        /// Gets or sets the regular expression string used to
        /// identify matching to a branch to identify branch matches.
        /// </summary>
        public string Match { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label parts to override in the generated version.
        /// </summary>
        public List<string> Label { get; set; }

        /// <summary>
        /// Gets or sets the label parts to insert at the start of the label in the generated version.
        /// </summary>
        public List<string> PrefixLabel { get; set; }

        /// <summary>
        /// Gets or sets the label parts to insert at the end of the label in the generated version.
        /// </summary>
        public List<string> PostfixLabel { get; set; }

        /// <summary>
        /// Gets or sets the label parts to insert at specific indexes of the label in the generated version.
        /// </summary>
        public Dictionary<int, string> InsertLabel { get; set; }

        /// <summary>
        /// Gets or sets the metadata parts to use in the generated version.
        /// </summary>
        public List<string> Metadata { get; set; }

        /// <summary>
        /// Gets or sets the metadata parts to insert at the start of the metadata in the generated version.
        /// </summary>
        public List<string> PrefixMetadata { get; set; }

        /// <summary>
        /// Gets or sets the metadata parts to insert at the end of the metadata in the generated version.
        /// </summary>
        public List<string> PostfixMetadata { get; set; }

        /// <summary>
        /// Gets or sets the metadata parts to insert at specific indexes of the metadata in the generated version.
        /// </summary>
        public Dictionary<int, string> InsertMetadata { get; set; }
    }
}
