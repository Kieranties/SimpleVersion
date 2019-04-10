// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;

namespace SimpleVersion.Model
{
    public class BranchConfiguration
    {
        public string Match { get; set; } = string.Empty;

#pragma warning disable CA2227 // Collection properties should be read only
        public List<string> Label { get; set; } = null;

        public List<string> MetaData { get; set; } = null;
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
