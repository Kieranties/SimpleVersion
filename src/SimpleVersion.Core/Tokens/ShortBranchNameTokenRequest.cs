// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("shortbranchname", Description = "Provides parsing of the branch name.")]
    public class ShortBranchNameTokenRequest : BranchNameTokenRequest
    {
        public ShortBranchNameTokenRequest()
        {
            base.BranchName = BranchNameOption.Short;
        }

        public override BranchNameOption BranchName
        {
            get => base.BranchName;
            set => throw new InvalidOperationException($"{nameof(ShortBranchNameTokenRequest)} does not support changing options.");
        }

        public override void Parse(string optionValue)
        {
            if (!string.IsNullOrWhiteSpace(optionValue))
            {
                throw new InvalidOperationException($"{nameof(ShortBranchNameTokenRequest)} does not support options.");
            }
        }
    }
}
