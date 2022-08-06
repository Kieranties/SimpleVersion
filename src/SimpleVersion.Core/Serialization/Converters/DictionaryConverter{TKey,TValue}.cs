// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleVersion.Serialization.Converters
{
    /// <summary>
    /// Enables reading/writing a dictionary as JSON.
    /// Implemented to support reading/writing non-standard keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class DictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue?>>
        where TKey : notnull
    {
        /// <inheritdoc />
        public override Dictionary<TKey, TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException(Resources.Exception_InvalidJsonToken(reader.TokenType, this.GetType()));
            }

            // step forward
            reader.Read();

            var instance = new Dictionary<TKey, TValue?>();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var (key, value) = ReadEntry(ref reader, options);
                instance.Add(key, value);
            }

            return instance;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue?> value, JsonSerializerOptions options)
        {
            Assert.ArgumentNotNull(writer, nameof(writer));
            Assert.ArgumentNotNull(value, nameof(value));

            writer.WriteStartObject();
            foreach (var entry in value)
            {
                writer.WritePropertyName(JsonSerializer.Serialize(entry.Key, options));
                JsonSerializer.Serialize(writer, entry.Value, options);
            }

            writer.WriteEndObject();
        }

        private (TKey Key, TValue? Value) ReadEntry(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException(Resources.Exception_InvalidJsonToken(reader.TokenType, this.GetType()));
            }

            var key = JsonSerializer.Deserialize<TKey>(reader.ValueSpan, options);

            if (key == null)
            {
                throw new JsonException(Resources.Exception_InvalidJsonToken(reader.TokenType, this.GetType()));
            }

            reader.Read();

            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            reader.Read();

            return (key, value);
        }
    }
}
