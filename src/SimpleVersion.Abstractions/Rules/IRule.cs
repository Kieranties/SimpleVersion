// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System.Collections.Generic;

namespace SimpleVersion.Rules
{
    public interface IRule<T>
    {
        string Token { get; }

        T Resolve(VersionContext context, T value);

        IEnumerable<T> Apply(VersionContext context, IEnumerable<T> value);
    }
}
