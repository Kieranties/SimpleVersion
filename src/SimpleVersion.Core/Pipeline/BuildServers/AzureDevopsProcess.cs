// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Text.RegularExpressions;

namespace SimpleVersion.Pipeline.BuildServers
{
    public class AzureDevopsProcess : ICalculatorProcess
    {
        private static readonly Regex _trim = new Regex(@"^refs\/(heads\/)?", RegexOptions.IgnoreCase);
        private readonly IEnvironment _env;

        public AzureDevopsProcess() : this(new VersioningEnvironment())
        {

        }

        public AzureDevopsProcess(IEnvironment env)
        {
            _env = env;
        }

        public void Apply(VersionContext context)
        {
            if (_env.GetVariable("TF_BUILD").ToBool())
            {
                context.Result.CanonicalBranchName = _env.GetVariable("BUILD_SOURCEBRANCH");
                context.Result.BranchName = _trim.Replace(context.Result.CanonicalBranchName, string.Empty); ;
            }
        }
    }
}
