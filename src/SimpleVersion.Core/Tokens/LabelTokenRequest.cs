// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Tokens
{
    [Token("label", Description = "Provides parsing of the version label.")]
    public class LabelTokenRequest : ITokenRequest
    {
        public string Separator { get; set; } = ".";

        public void Parse(string optionValue) => this.Separator = optionValue ?? string.Empty;
    }
}
