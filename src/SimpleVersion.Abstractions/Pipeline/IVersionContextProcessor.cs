// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Abstractions.Pipeline
{
    /// <summary>
    /// Contract for a processor step during version calculation.
    /// </summary>
    /// <typeparam name="TContext">The type of context that can be processed.</typeparam>
    public interface IVersionContextProcessor<TContext> where TContext : IVersionContext
    {
        /// <summary>
        /// Applies the processor to the given context.
        /// </summary>
        /// <param name="context">The context of the version calculation.</param>
        void Apply(TContext context);
    }
}
