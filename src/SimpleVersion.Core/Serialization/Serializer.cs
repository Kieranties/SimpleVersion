// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Text.Encodings.Web;
using System.Text.Json;
using SimpleVersion.Serialization.Converters;

namespace SimpleVersion.Serialization
{
    /// <summary>
    /// Supports serialization to/from JSON.
    /// </summary>
    public class Serializer : ISerializer
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = false,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new DictionaryConverter(),
                new IntConverter()
            }
        };

        /// <inheritdoc />
        public T? Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, _options);

        /// <inheritdoc />
        public string Serialize(object value) => JsonSerializer.Serialize(value, _options);
    }
}
