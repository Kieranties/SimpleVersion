// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Abstractions.Exceptions
{
    /// <summary>
    /// Represents a Git error within the application.
    /// </summary>
    public class GitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitException"/> class.
        /// </summary>
        public GitException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitException"/> class.
        /// </summary>
        /// <param name="message">The user message.</param>
        public GitException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">An <see cref="Exception"/> to be wrapped.</param>
        public GitException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
