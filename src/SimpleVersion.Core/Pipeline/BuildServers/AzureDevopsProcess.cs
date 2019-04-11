// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Text.RegularExpressions;

namespace SimpleVersion.Pipeline.BuildServers
{
    /// <summary>
    /// Modifies the calculation when running in TFS/Azure Devops.
    /// </summary>
    public class AzureDevopsProcess : IVersionProcessor
    {
        private static readonly Regex _trim = new Regex(@"^refs\/(heads\/)?", RegexOptions.IgnoreCase);
        private readonly IEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDevopsProcess"/> class.
        /// </summary>
        public AzureDevopsProcess() : this(new VersioningEnvironment())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDevopsProcess"/> class.
        /// </summary>
        /// <param name="env">The <see cref="IEnvironment"/> for accessing environment variables.</param>
        public AzureDevopsProcess(IEnvironment env)
        {
            _env = env;
        }

        /// <inheritdoc/>
        public void Apply(VersionContext context)
        {
            if (_env.GetVariable("TF_BUILD").ToBool())
            {
                context.Result.CanonicalBranchName = _env.GetVariable("BUILD_SOURCEBRANCH");
                context.Result.BranchName = _trim.Replace(context.Result.CanonicalBranchName, string.Empty);
            }
        }
    }
}
