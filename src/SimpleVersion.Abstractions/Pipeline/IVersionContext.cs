// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;

namespace SimpleVersion.Abstractions.Pipeline
{
    /// <summary>
    /// Encapsulates state during version calculation.
    /// </summary>
    public interface IVersionContext
    {
        /// <summary>
        /// Gets or sets the resolved settings for the repository.
        /// </summary>
        Settings Settings { get; set; }

        /// <summary>
        /// Gets or sets the result of the version calculation.
        /// </summary>
        VersionResult Result { get; set; }
    }
}
