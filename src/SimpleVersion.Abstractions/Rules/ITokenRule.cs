// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System.Collections.Generic;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Contract for a rule which applies for a specific token.
    /// </summary>
    /// <typeparam name="T">The type affected by the rule.</typeparam>
    public interface ITokenRule<T>
    {
        /// <summary>
        /// Gets the token string.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Resolve the <typeparamref name="T"/> value using the given context.
        /// The result will have any application of the token replaced.
        /// </summary>
        /// <param name="context">The <see cref="VersionContext"/> of the current calculation.</param>
        /// <param name="value">The <typeparamref name="T"/> value to resolve the token against.</param>
        /// <returns>An instance of <typeparamref name="T"/> with the tokens replaced.</returns>
        T Resolve(VersionContext context, T value);

        /// <summary>
        /// Returns an enumerables of <typeparamref name="T"/> where the rule token
        /// may have been applied of the token rule requires it.
        /// </summary>
        /// <param name="context">The <see cref="VersionContext"/> of the current calculation.</param>
        /// <param name="value">An enumerable of <typeparamref name="T"/> values to possible update.</param>
        /// <returns>An enuemrable of <typeparamref name="T"/> where inclusion of the token may have been applied.</returns>
        IEnumerable<T> Apply(VersionContext context, IEnumerable<T> value);
    }
}
