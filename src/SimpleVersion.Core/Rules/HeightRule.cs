// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleVersion.Rules
{
	public class HeightRule : IRule<string>
	{
		private static readonly Lazy<HeightRule> _default = new Lazy<HeightRule>(() => new HeightRule());

		public static HeightRule Instance => _default.Value;

		public HeightRule() : this(false)
		{

		}

		public HeightRule(bool padded)
		{
			Padded = padded;
		}

		public bool Padded { get; }

		public string Token => "*";

		public string Resolve(VersionContext context, string value)
		{
			if (Padded)
				return value.Replace(Token, context.Result.HeightPadded);
			else
				return value.Replace(Token, context.Result.Height.ToString(System.Globalization.CultureInfo.CurrentCulture));
		}

		public IEnumerable<string> Apply(VersionContext context, IEnumerable<string> input)
		{
			if (!context.Configuration.Version.Contains(Token)
				&& input.Count() != 0
				&& !input.Contains(Token))
			{
				return input.Concat(new[] { Token });
			}

			return input;
		}
	}
}
