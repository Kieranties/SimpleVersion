// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("pr", Description = "Provides parsing of the pull-request number.")]
    public class PrTokenRequest : ITokenRequest
    {
        public void Parse(string optionValue) => throw new InvalidOperationException("Option values not supported.");
    }
}
