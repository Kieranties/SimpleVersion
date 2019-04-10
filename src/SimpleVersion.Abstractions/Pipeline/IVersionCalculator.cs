// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;

namespace SimpleVersion.Pipeline
{
	public interface IVersionCalculator
	{
		IVersionCalculator AddProcessor<T>() where T : ICalculatorProcess, new();

		VersionResult GetResult(string path);
	}
}