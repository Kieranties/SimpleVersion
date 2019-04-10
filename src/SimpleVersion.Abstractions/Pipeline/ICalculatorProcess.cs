// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Pipeline
{
    public interface ICalculatorProcess
    {
        void Apply(VersionContext context);
    }
}