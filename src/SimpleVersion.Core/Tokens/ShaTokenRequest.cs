// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Tokens
{
    [Token("sha", Description = "Provides parsing of the commit sha.")]
    public class ShaTokenRequest : ITokenRequest
    {
        private int _length = (int)ShaLengthOption.Full;

        // TODO: handle 'short', 'full', greater than 0;
        public virtual int Length
        {
            get => _length;
            set => SetLengthIfValid(value);
        }

        public virtual void Parse(string optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionValue))
            {
                SetLengthIfValid((int)ShaLengthOption.Full);
                return;
            }

            if (Enum.TryParse<ShaLengthOption>(optionValue, ignoreCase: true, out var enumResult))
            {
                SetLengthIfValid((int)enumResult);
                return;
            }

            if (int.TryParse(optionValue, out var result))
            {
                SetLengthIfValid(result);
            }
            else
            {
                throw new InvalidOperationException("Invalid value.");
            }
        }

        public void SetLengthIfValid(int value)
        {
            if (value < 1)
            {
                throw new InvalidOperationException("Must be greater than zero.");
            }

            value = Math.Min((int)ShaLengthOption.Full, value);

            _length = value;
        }
    }
}
