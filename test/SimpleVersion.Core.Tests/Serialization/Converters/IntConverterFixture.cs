// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Buffers.Text;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using SimpleVersion.Serialization.Converters;
using Xunit;

namespace SimpleVersion.Core.Tests.Serialization.Converters
{
    public class IntConverterFixture
    {
        private readonly IntConverter _sut;

        public IntConverterFixture()
        {
            _sut = new IntConverter();
        }

        [Fact]
        public void Read_Number_ReturnsNumber()
        {
            // Arrange
            var reader = GetReader(new { Number = 20 });
            reader.Read(); // start token
            reader.Read(); // property name
            reader.Read(); // property value;

            // Act
            var result = _sut.Read(ref reader, null, null);

            // Assert
            result.Should().Be(20);
        }

        [Theory]
        [InlineData("0002", 2)]
        [InlineData("0200", 200)]
        [InlineData("20", 20)]
        public void Read_String_ReturnsNumber(string value, int expected)
        {
            // Arrange
            var reader = GetReader(new { Number = value });
            reader.Read(); // start token
            reader.Read(); // property name
            reader.Read(); // property value;

            // Act
            var result = _sut.Read(ref reader, null, null);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Read_UnexpectedTokenType_Throws()
        {
            // Arrange
            var reader = GetReader(new { Number = new[] { 1, 2, 3, 4 } });
            reader.Read(); // start token
            reader.Read(); // property name
            reader.Read(); // property value;

            try
            {
                // Act
                _sut.Read(ref reader, null, null);
            }
            catch (JsonException ex)
            {
                // Assert
                ex.Message.Should().Be("Unexpected JsonToken 'StartArray' in converter SimpleVersion.Serialization.Converters.IntConverter.");
            }
        }

        [Fact]
        public void Write_NullWriter_Throws()
        {
            // Arrange
            Action action = () => _sut.Write(null, 1, null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("writer");
        }

        [Fact]
        public void Write_WithWriter_WritesValue()
        {
            // Arrange
            var value = 50;
            var expected = new byte[2];
            Utf8Formatter.TryFormat(value, new Span<byte>(expected), out var _);

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _sut.Write(writer, value, null);

            // Assert
            writer.Flush();
            var result = stream.ToArray();
            result.Should().BeEquivalentTo(expected);
        }

        private Utf8JsonReader GetReader<T>(T value)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, typeof(T));
            var span = new ReadOnlySpan<byte>(bytes);
            return new Utf8JsonReader(span);
        }
    }
}
