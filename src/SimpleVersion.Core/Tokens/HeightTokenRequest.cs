// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("*", Description = "Provides parsing of the commit height.")]
    public class HeightTokenRequest : ITokenRequest
    {
        private int _padding = 0;

        public int Padding
        {
            get => _padding;
            set => SetPaddingIfValid(value);
        }

        public void Parse(string optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionValue))
            {
                Padding = 0;
                return;
            }

            if (int.TryParse(optionValue, out var result))
            {
                SetPaddingIfValid(result);
            }
            else
            {
                throw new InvalidOperationException("Invalid value");
            }
        }

        private void SetPaddingIfValid(int value)
        {
            if (value < 0)
            {
                throw new InvalidOperationException("Value must be zero or greater");
            }

            _padding = value;
        }
    }
}
