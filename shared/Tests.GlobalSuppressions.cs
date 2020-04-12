// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Use underscores for readability in tests")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleCop.CSharp.OrderingRules",
    "SA1204:Static elements should appear before instance elements",
    Justification = "Improves readability in tests")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "Tests do not require documentation")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Usage",
    "CA1806:Do not ignore method results",
    Justification = "Tests use lambdas/anonymous functions for exception handling")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Usage",
    "CA1062:Validate arguments of public methods",
    Justification = "Tests do not require validation")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "Tests do not require enforcement of statics")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Globalization",
    "CA1303:Do not pass literals as localized parameters",
    Justification = "Simplifies tests")]
