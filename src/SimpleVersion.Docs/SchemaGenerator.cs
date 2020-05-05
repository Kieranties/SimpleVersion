// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;

namespace SimpleVersion.Docs
{
    /// <summary>
    /// Generates JsonSchemas.
    /// </summary>
    internal static class SchemaGenerator
    {
        /// <summary>
        /// Generates a JsonSchema for the given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to generate a schema for.</param>
        /// <param name="path">The output path of the schema.</param>
        internal static void Generate(Type type, string path)
        {
            var settings = new JsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true,
                SerializerSettings = new JsonSerializerSettings
                {
                  ContractResolver = new DefaultContractResolver
                  {
                      NamingStrategy = new CamelCaseNamingStrategy()
                  }
                }
            };

            var schema = JsonSchema.FromType(type, settings);
            Directory.CreateDirectory(Directory.GetParent(path).FullName);
            File.WriteAllText(path, schema.ToJson());
        }
    }
}
