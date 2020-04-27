// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;

namespace SimpleVersion.Abstractions.Tests
{
    public static class AssertUtils
    {
        public static void AssertGetSetProperty<TSubject, TProperty>(
            TSubject instance,
            string propertyName,
            Action<TProperty> getAssert,
            TProperty setValue)
        {
            var property = typeof(TSubject).GetProperty(propertyName);

            // Assert Get
            var value = (TProperty)property.GetValue(instance);
            getAssert(value);

            // Assert Set
            property.SetValue(instance, setValue);
            var newValue = (TProperty)property.GetValue(instance);
            newValue.Should().Be(setValue);
        }
    }
}
