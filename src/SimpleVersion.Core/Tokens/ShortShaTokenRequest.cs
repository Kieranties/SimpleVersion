// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("shortsha", Description = "Provides parsing of the commit sha.")]
    public class ShortShaTokenRequest : ShaTokenRequest
    {
        public ShortShaTokenRequest()
        {
            base.Length = (int)ShaLengthOption.Short;
        }

        public override int Length
        {
            get => base.Length;
            set => throw new InvalidOperationException($"{nameof(ShortShaTokenRequest)} does not support changing 'Length'.");
        }

        public override void Parse(string optionValue)
        {
            if (!string.IsNullOrWhiteSpace(optionValue))
            {
                throw new InvalidOperationException($"{nameof(ShortShaTokenRequest)} does not support options.");
            }
        }
    }
}
