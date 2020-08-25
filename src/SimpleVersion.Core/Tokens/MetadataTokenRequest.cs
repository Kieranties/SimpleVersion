// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Tokens
{

    [Token("metadata", Description = "Provides parsing of the version metadata.")]
    public class MetadataTokenRequest : ITokenRequest
    {
        public string Separator { get; set; } = ".";

        public void Parse(string optionValue) => this.Separator = optionValue ?? string.Empty;
    }
}
