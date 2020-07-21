// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles the formatting of the version string.
    /// </summary>
    [TokenValueOption(_majorOption, Description = "Returns the major part of the version string.")]
    [TokenValueOption(_minorOption, Description = "Returns the minor part of the version string.")]
    [TokenValueOption(_patchOption, Description = "Returns the patch part of the version string.")]
    [TokenValueOption(_revisionOption, Description = "Returns the revision part of the version string.")]
    [TokenValueOption(_revisionIfSetOption, Description = "Returns the revision part of the version string if it is set in configuration.")]
    [TokenValueOption(_defaultOption, Description = "Returns the major.minor.patch version string with revision if set in configuration.")]
    [TokenFallbackOption("Provide any combination of the option values.")]
    public class VersionToken : ITokenRequestHandler<VersionTokenRequest>
    {
        private const string _majorOption = "M";
        private const string _minorOption = "m";
        private const string _patchOption = "p";
        private const string _revisionIfSetOption = "r";
        private const string _revisionOption = "R";
        private const string _defaultOption = "Mmpr";

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
        public string Format { get; set; } = "Mmpr";

        public void Parse(string optionValue)
        {

        }
    }
}
