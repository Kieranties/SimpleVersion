// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleVersion.Serialization.Converters
{
    /// <summary>
    /// Enables reading/writing a dictionary as JSON.
    /// Implemnted to support reading/writing non-standard keys.
    /// </summary>
    public class DictionaryConverter : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            var definition = typeToConvert.GetGenericTypeDefinition();

            return definition == typeof(IDictionary<,>)
                || definition == typeof(Dictionary<,>);
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(DictionaryConverter<,>).MakeGenericType(typeToConvert.GetGenericArguments());
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
