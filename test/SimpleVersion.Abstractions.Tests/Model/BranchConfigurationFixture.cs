// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Model;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Model
{
	public class BranchConfigurationFixture
	{
		[Fact]
		public void Ctor_SetsDefaults()
		{
			// Arrange / Act
			var sut = new BranchConfiguration();

			// Assert
			sut.Label.Should().BeNull();
			sut.Match.Should().BeEmpty();
			sut.MetaData.Should().BeNull();
		}
	}
}
