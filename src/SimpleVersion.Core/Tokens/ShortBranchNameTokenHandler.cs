using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVersion.Tokens
{
    public class ShortBranchNameTokenHandler : BranchNameTokenHandler
    {
        public override string Key => "shortbranchname";

        public override string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            return evaluator.Process("{branchname:short}", context);
        }
    }
}
