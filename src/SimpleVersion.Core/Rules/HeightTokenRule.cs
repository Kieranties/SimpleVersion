// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the height.
    /// </summary>
    public class HeightTokenRule : ITokenRule<string>
    {
        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static HeightTokenRule Instance => _default.Value;

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
        /// Gets a value indicating whether padding should be applied.
        /// </summary>
        public bool Padded { get; }

        /// <inheritdoc/>
        public string Token => "*";

        /// <inheritdoc/>
        public string Resolve(VersionContext context, string value)
        {
            if (Padded)
                return value.Replace(Token, context.Result.HeightPadded);
            else
                return value.Replace(Token, context.Result.Height.ToString(System.Globalization.CultureInfo.CurrentCulture));
        }

        /// <inheritdoc/>
        public IEnumerable<string> Apply(VersionContext context, IEnumerable<string> input)
        {
            if (!context.Configuration.Version.Contains(Token)
                && input.Count() != 0
                && !input.Any(x => x.Contains(Token)))
            {
                return input.Concat(new[] { Token });
            }

            return input;
        }
    }
}
