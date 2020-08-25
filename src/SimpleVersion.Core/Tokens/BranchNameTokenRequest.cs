// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("branchname", Description = "Provides parsing of the branch name.")]
    public class BranchNameTokenRequest : ITokenRequest
    {
        public virtual BranchNameOption BranchName { get; set; } = BranchNameOption.Canonical;

        public virtual void Parse(string optionValue)
        {
            if (Enum.TryParse<BranchNameOption>(optionValue, ignoreCase: true, out var result))
            {
                this.BranchName = result;
            }
            else
            {
                throw new ArgumentException($"Invalid option '{optionValue}' - Valid options are 'canonical', 'short', 'suffix'.");
            }
        }
    }
}
