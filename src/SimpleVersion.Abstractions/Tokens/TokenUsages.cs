// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Identifies the areas in which a token may be used.
    /// </summary>
    [Flags]
    public enum TokenUsages
    {
        /// <summary>
        /// The token may be used in the version string.
        /// </summary>
        Version = 0,

        /// <summary>
        /// The token may be used in the label string.
        /// </summary>
        Label = 1,

        /// <summary>
        /// The token may be used in the metadata string
        /// </summary>
        Metadata = 2,

        /// <summary>
        /// The token may be used in any string.
        /// </summary>
        Any = Version + Label + Metadata
    }
}
