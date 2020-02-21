// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using SimpleVersion.Serialization.Converters;

namespace SimpleVersion.Serialization
{
    /// <summary>
    /// Supports serialization to/from JSON.
    /// </summary>
    public static class Serializer
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = false,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new DictionaryConverter(),
                new IntConverter()
            }
        };

        /// <summary>
        /// Returns an instance of <typeparamref name="T"/> based on the given <paramref name="json"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to be returned.</typeparam>
        /// <param name="json">The JSON value of the object.</param>
        /// <returns>An instance of <typeparamref name="T"/> with the values of <paramref name="json"/>.</returns>
        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);

        /// <summary>
        /// Returns a JSON string of the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The object to be serialized.</param>
        /// <returns>The JSON string.</returns>
        public static string Serialize(object value) => JsonSerializer.Serialize(value, _options);
    }
}
