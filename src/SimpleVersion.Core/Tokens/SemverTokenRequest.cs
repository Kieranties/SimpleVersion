// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("semver", Description = "Provides parsing of full semver compatible versions.")]
    public class SemverTokenRequest : ITokenRequest
    {
        private int _version = 2;

        public int Version
        {
            get => _version;
            set => SetVersionIfValid(value);
        }

        public void Parse(string optionValue)
        {
            if (int.TryParse(optionValue, out var result))
            {
                SetVersionIfValid(result);
            }
            else
            {
                throw new InvalidOperationException("Invalid value");
            }
        }

        private void SetVersionIfValid(int value)
        {
            if (value < 1 || value > 2)
            {
                throw new InvalidOperationException("Value must be 1 or 2.");
            }

            _version = value;
        }
    }
}
