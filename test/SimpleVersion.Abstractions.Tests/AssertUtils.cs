// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;

namespace SimpleVersion.Abstractions.Tests
{
    public static class AssertUtils
    {
        public static void AssertGetSetProperty<T, K>(
            T instance,
            string propertyName,
            Action<K> getAssert,
            K setValue)
        {
            var property = typeof(T).GetProperty(propertyName);

            // Assert Get
            var value = (K)property.GetValue(instance);
            getAssert(value);

            // Assert Set
            property.SetValue(instance, setValue);
            var newValue = (K)property.GetValue(instance);
            newValue.Should().Be(setValue);
        }
    }
}
