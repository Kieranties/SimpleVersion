// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Text.RegularExpressions;
using SimpleVersion.Extensions;

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Represents the environment when running on Azure Devops.
    /// </summary>
    public class AzureDevopsEnvironment : BaseVersionEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDevopsEnvironment"/> class.
        /// </summary>
        /// <param name="accessor">The accessor for environment variables.</param>
        public AzureDevopsEnvironment(IEnvironmentVariableAccessor accessor)
            : base(accessor)
        {
            if (string.IsNullOrWhiteSpace(CanonicalBranchName))
            {
                CanonicalBranchName = Variables.GetVariable("BUILD_SOURCEBRANCH");
                if (CanonicalBranchName != null)
                {
                    BranchName = CanonicalBranchTrimmer.Replace(CanonicalBranchName, string.Empty);
                }
            }
        }

        /// <inheritdoc/>
        public override bool IsValid => Variables.GetVariable("TF_BUILD").ToBool();
    }
}
