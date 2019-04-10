// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline.Formatting
{
	public class Semver2FormatProcessFixture
	{
		private readonly Semver2FormatProcess _sut;

		public Semver2FormatProcessFixture()
		{
			_sut = new Semver2FormatProcess();
		}

		public static IEnumerable<object[]> LabelParts()
		{
			return new List<object[]>
			{
				new object[] { Array.Empty<object>(), "1.2.0", 10 },
				new object[] { new[] { "one" }, "1.2.0", 10 },
				new object[] { new[] { "one", "two" }, "1.2.0", 106 }
			};
		}

		[Theory]
		[MemberData(nameof(LabelParts))]
		public void Apply_LabelParts_NonRelease_Is_Formatted(
			string[] parts,
			string version,
			int height)
		{
			// Arrange
			var context = new VersionContext
			{
				Configuration = Utils.GetConfiguration(version, label: parts),
				Result = Utils.GetVersionResult(height, false)
			};
			context.Result.Version = context.Configuration.Version;

			string expected;
			if (parts.Length > 0)
				expected = $"{version}-{string.Join(".", parts)}.{height}.c4ca82d2";
			else
				expected = $"{version}-c4ca82d2";

			// Act
			_sut.Apply(context);

			// Assert
			context.Result.Formats.Should().ContainKey("Semver2");
			context.Result.Formats["Semver2"].Should().Be(expected);
		}


		[Theory]
		[MemberData(nameof(LabelParts))]
		public void Apply_LabelParts_Release_Is_Formatted(
			string[] parts,
			string version,
			int height)
		{
			// Arrange
			var context = new VersionContext
			{
				Configuration = Utils.GetConfiguration(version, parts),
				Result = Utils.GetVersionResult(height)
			};
			context.Result.Version = context.Configuration.Version;

			string expected;
			if (parts.Length > 0)
				expected = $"{version}-{string.Join(".", parts)}.{height}";
			else
				expected = $"{version}";

			// Act
			_sut.Apply(context);

			// Assert
			context.Result.Formats.Should().ContainKey("Semver2");
			context.Result.Formats["Semver2"].Should().Be(expected);
		}

		public static IEnumerable<object[]> MetaDataParts()
		{
			yield return new object[] { Array.Empty<object>(), "1.2.0", 10 };
			yield return new object[] { new[] { "one" }, "1.2.0", 10 };
			yield return new object[] { new[] { "one", "two" }, "1.2.0", 106 };
		}

		[Theory]
		[MemberData(nameof(MetaDataParts))]
		public void Apply_MetaDataParts_NonRelease_Is_Formatted(
			string[] parts,
			string version,
			int height)
		{
			// Arrange
			var context = new VersionContext
			{
				Configuration = Utils.GetConfiguration(version, meta: parts),
				Result = Utils.GetVersionResult(height, false)
			};
			context.Result.Version = context.Configuration.Version;

			string expected;
			if (parts.Length > 0)
				expected = $"{version}-c4ca82d2+{string.Join(".", parts)}";
			else
				expected = $"{version}-c4ca82d2";

			// Act
			_sut.Apply(context);

			// Assert
			context.Result.Formats.Should().ContainKey("Semver2");
			context.Result.Formats["Semver2"].Should().Be(expected);
		}

		[Theory]
		[MemberData(nameof(MetaDataParts))]
		public void Apply_MetaDataParts_Release_Is_Formatted(
			string[] parts,
			string version,
			int height)
		{

			// Arrange
			var context = new VersionContext
			{
				Configuration = Utils.GetConfiguration(version, meta: parts),
				Result = Utils.GetVersionResult(height)
			};
			context.Result.Version = context.Configuration.Version;

			string expected;
			if (parts.Length > 0)
				expected = $"{version}+{string.Join(".", parts)}";
			else
				expected = $"{version}";

			// Act
			_sut.Apply(context);

			// Assert
			context.Result.Formats.Should().ContainKey("Semver2");
			context.Result.Formats["Semver2"].Should().Be(expected);
		}
	}
}
