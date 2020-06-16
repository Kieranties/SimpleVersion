// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles token replacement for height.
    /// </summary>
    public class HeightTokenHandler : ITokenHandler
    {
        /// <inheritdoc />
        public string Key => "*";

        /// <inheritdoc />
        public string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            var height = context.Result.Height.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(optionValue))
            {
                if (int.TryParse(optionValue, out var padding))
                {
                    height = height.PadLeft(padding, '0');
                }
                else
                {
                    throw new InvalidOperationException($"Invalid option for height token: '{optionValue}'");
                }
            }

            return height;
        }
    }
}
