// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Abstractions.Rules;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the height.
    /// </summary>
    public class HeightTokenRule : ITokenRule<string>
    {
        private static readonly Lazy<HeightTokenRule> _default = new Lazy<HeightTokenRule>(() => new HeightTokenRule());

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightTokenRule"/> class.
        /// </summary>
        public HeightTokenRule() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightTokenRule"/> class.
        /// </summary>
        /// <param name="padded">True if the value should be padded.</param>
        public HeightTokenRule(bool padded)
        {
            Padded = padded;
        }

        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static HeightTokenRule Instance => _default.Value;

        /// <summary>
        /// Gets a value indicating whether padding should be applied.
        /// </summary>
        public bool Padded { get; }

        /// <inheritdoc/>
        public string Token => "*";

        /// <inheritdoc/>
        public string Resolve(IVersionContext context, string value)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(value, nameof(value));

            if (Padded)
            {
                return value.Replace(Token, context.Result.HeightPadded, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return value.Replace(Token, context.Result.Height.ToString(System.Globalization.CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Apply(IVersionContext context, IEnumerable<string> input)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (!context.Settings.Version.Contains(Token, StringComparison.OrdinalIgnoreCase))
            {
                var inputEntries = input.ToArray();
                if (inputEntries.Length > 0 && !inputEntries.Any(x => x.Contains(Token, StringComparison.OrdinalIgnoreCase)))
                {
                    return input.Concat(new[] { Token });
                }
            }

            return input;
        }
    }
}
