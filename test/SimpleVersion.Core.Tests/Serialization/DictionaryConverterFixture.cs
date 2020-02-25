// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Serialization.Converters;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Serialization
{
    public class DictionaryConverterFixture
    {
        private readonly DictionaryConverter _sut;

        public DictionaryConverterFixture()
        {
            _sut = new DictionaryConverter();
        }

        [Fact]
        public void CanConvert_NullType_Throws()
        {
            // Arrange
            Action action = () => _sut.CanConvert(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("typeToConvert");
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(List<string>), false)]
        [InlineData(typeof(Dictionary<int, string>), true)]
        [InlineData(typeof(Dictionary<string, string>), false)]
        [InlineData(typeof(IDictionary<int, string>), true)]
        [InlineData(typeof(IDictionary<string, string>), false)]
        public void CanConvert_ReturnsExpected(Type type, bool expected)
        {
            // Arrange / Act
            var result = _sut.CanConvert(type);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void CreateConverter_NullType_Throws()
        {
            // Arrange
            Action action = () => _sut.CreateConverter(null, null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("typeToConvert");
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(Dictionary<string, string>))]
        [InlineData(typeof(IDictionary<string, string>))]
        public void CreateConverter_NonConvertibleType_Throws(Type type)
        {
            // Arrange
            Action action = () => _sut.CreateConverter(type, null);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"{type} is not a valid type for converter {typeof(DictionaryConverter<,>)}");
        }

        [Theory]
        [InlineData(typeof(Dictionary<int, string>), typeof(int), typeof(string))]
        [InlineData(typeof(IDictionary<int, string>), typeof(int), typeof(string))]
        public void CreateConverter_ConvertibleType_ReturnsConverter(Type type, Type keyType, Type valueType)
        {
            // Arrange
            var expectedType = typeof(DictionaryConverter<,>).MakeGenericType(keyType, valueType);

            // Act
            var result = _sut.CreateConverter(type, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(expectedType);
        }
    }
}
