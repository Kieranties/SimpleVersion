// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Text.RegularExpressions;
using SimpleVersion.Extensions;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Modifies the calculation when running in TFS/Azure Devops.
    /// </summary>
    public class AzureDevopsContextProcessor : IVersionContextProcessor
    {
        private static readonly Regex _trim = new Regex(@"^refs\/(heads\/)?", RegexOptions.IgnoreCase);
        private readonly IVersionEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDevopsContextProcessor"/> class.
        /// </summary>
        public AzureDevopsContextProcessor() : this(new VersionEnvironment())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDevopsContextProcessor"/> class.
        /// </summary>
        /// <param name="env">The <see cref="IVersionEnvironment"/> for accessing environment variables.</param>
        public AzureDevopsContextProcessor(IVersionEnvironment env)
        {
            _env = env;
        }

        /// <inheritdoc/>
        public void Apply(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (_env.GetVariable("TF_BUILD").ToBool())
            {
                context.Result.CanonicalBranchName = _env.GetVariable("BUILD_SOURCEBRANCH");
                context.Result.BranchName = _trim.Replace(context.Result.CanonicalBranchName, string.Empty);
            }
        }
    }
}
