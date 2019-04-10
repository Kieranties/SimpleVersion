// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Rules
{
	public class ShortBranchNameRuleFixture
	{
		[Fact]
		public void Instance_SetsDefaults()
		{
			// Arrange / Act
			var sut = ShortBranchNameRule.Instance;

			// Assert
			sut.Pattern.Should().NotBeNull();
			sut.Token.Should().Be("{shortbranchname}");
		}

		public static IEnumerable<object[]> ApplyData()
		{
			yield return new object[] { null, null };
			yield return new object[] { null, Array.Empty<string>() };
			yield return new object[] { new VersionContext(), new[] { "this" } };
		}

		[Theory]
		[MemberData(nameof(ApplyData))]
		public void Apply_Returns_Input(VersionContext context, IEnumerable<string> input)
		{
			// Arrange
			var sut = new ShortBranchNameRule();

			// Act
			var result = sut.Apply(context, input);

			// Assert
			result.Should().BeSameAs(input);
		}

		[Theory]
		[InlineData("master", "{shortBranchName}", "master")]
		[InlineData("master", "{SHORTBRANCHNAME}", "master")]
		[InlineData("release/1.0", "{shortBRANCHNAME}", "release10")]
		[InlineData("release-1.0", "{shortBRANCHNAME}", "release10")]
		public void Resolve_Replaces_BranchName(string branchName, string input, string expected)
		{
			// Arrange
			var sut = new ShortBranchNameRule();
			var context = new VersionContext
			{
				Result =
				{
					BranchName = branchName
				}
			};

			// Act
			var result = sut.Resolve(context, input);

			// Assert
			result.Should().Be(expected);
		}


		[Theory]
		[InlineData("master", "{shortbranchName}", "[mr]", "aste")]
		public void Resolve_CustomPattern_Replaces_BranchName(string branchName, string input, string pattern, string expected)
		{
			// Arrange
			var sut = new ShortBranchNameRule(pattern);
			var context = new VersionContext
			{
				Result =
				{
					BranchName = branchName
				}
			};

			// Act
			var result = sut.Resolve(context, input);

			// Assert
			result.Should().Be(expected);
		}
	}
}
