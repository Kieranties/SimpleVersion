// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using SimpleVersion.Pipeline;
using System.Linq;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles the formatting of the version string.
    /// </summary>
    public class VersionToken : ITokenRequestHandler<VersionTokenRequest>
    {
        private const string _majorOption = "M";
        private const string _minorOption = "m";
        private const string _patchOption = "p";
        private const string _revisionIfSetOption = "r";
        private const string _revisionOption = "R";

        /// <inheritdoc/>
        public string Evaluate(VersionTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(request, nameof(request));

            var versionParts = new List<int>();

            foreach (var c in request.Format)
            {
                int num;

                if (c == _revisionIfSetOption[0] && context.Result.Revision < 1)
                {
                    continue;
                }

                num = c.ToString() switch
                {
                    _majorOption => context.Result.Major,
                    _minorOption => context.Result.Minor,
                    _patchOption => context.Result.Patch,
                    _revisionIfSetOption => context.Result.Revision,
                    _revisionOption => context.Result.Revision,
                    _ => throw new InvalidOperationException($"Invalid character '{c}' in version option [{request.Format}].")
                };

                versionParts.Add(num);
            }

            return string.Join('.', versionParts);
        }
    }

    [Token("version", Description = "Provides parsing of the version string.")]
    public class VersionTokenRequest : ITokenRequest
    {
        private static readonly char[] _allowed = new[] { 'M', 'm', 'p', 'r', 'R' };

        public string Format { get; set; } = "Mmpr";

        public void Parse(string optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionValue))
            {
                return;
            }

            var set = new HashSet<char>();
            var valid = optionValue
                .ToHashSet()
                .All(c => _allowed.Contains(c));

            if(!valid)
            {
                throw new InvalidOperationException("Invalid value");
            }

        }
    }
}
