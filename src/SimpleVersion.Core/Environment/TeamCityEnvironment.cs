// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Text.RegularExpressions;
using SimpleVersion.Extensions;

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Represents the environment when running on Azure Devops.
    /// </summary>
    public class TeamCityEnvironment : BaseVersionEnvironment
    {
        private static readonly Regex _trim = new Regex(@"^refs\/(heads\/)?", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamCityEnvironment"/> class.
        /// </summary>
        /// <param name="accessor">The accessor for environment variables.</param>
        public TeamCityEnvironment(IEnvironmentVariableAccessor accessor)
            : base(accessor)
        {
            CanonicalBranchName = Variables.GetVariable("BUILD_SOURCEBRANCH");
            if (CanonicalBranchName != null)
            {
                BranchName = _trim.Replace(CanonicalBranchName, string.Empty);
            }
        }

        /// <inheritdoc/>
        public override string? CanonicalBranchName { get; }

        /// <inheritdoc/>
        public override string? BranchName { get; }

        /// <inheritdoc/>
        public override bool IsValid => string.IsNullOrWhiteSpace(Variables.GetVariable("TEAMCITY_VERSION")) ? false : true;
    }
}
