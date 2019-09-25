// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleVersion.Serialization.Converters
{
    /// <summary>
    /// Enables converting a JSON number or string as an int.
    /// </summary>
    public class IntConverter : JsonConverter<int>
    {
        /// <inheritdoc />
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt32();

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }

            throw new JsonException(Resources.Exception_InvalidJsonToken(reader.TokenType, this.GetType()));
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
