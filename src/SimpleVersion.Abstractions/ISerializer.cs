// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion
{
    /// <summary>
    /// Handles serialization.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the given <see cref="object"/>.
        /// </summary>
        /// <param name="value">The instance value.</param>
        /// <returns>A string representation of the given <paramref name="value"/>.</returns>
        string Serialize(object value);

        /// <summary>
        /// Deserialzies the given <paramref name="value"/> into an new
        /// instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="value">A string representation of <typeparamref name="T"/>.</param>
        /// <returns>An new instance of <typeparamref name="T"/> populated from <paramref name="value"/>.</returns>
        T Deserialize<T>(string value);
    }
}
