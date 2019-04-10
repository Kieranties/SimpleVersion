// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion
{
    public class VersioningEnvironment : IEnvironment
    {
        public string GetVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}
