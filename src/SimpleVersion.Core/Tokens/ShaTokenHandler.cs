// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Exposes the git sha as a token for consumption.
    /// </summary>
    public class ShaTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "sha";

        /// <inheritdoc/>
        public string Process(string? option, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            var result = context.Result.Sha;

            if (string.IsNullOrWhiteSpace(option))
            {
                return result;
            }

            if (int.TryParse(option, out var length))
            {
                if (length < 1)
                {
                    throw new InvalidOperationException($"Invalid sha substring length {option}.  Expected an integer greater than 0.");
                }
                else if (length >= result.Length)
                {
                    length = result.Length;
                }

                return result.Substring(0, length);
            }
            else
            {
                throw new InvalidOperationException($"Invalid sha substring length {option}.  Expected an integer greater than 0.");
            }
        }
    }
}
