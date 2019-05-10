SimpleVersion
=============
[![Nuget](https://img.shields.io/nuget/v/SimpleVersion.Core.svg?logo=nuget)][NugetRel]
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SimpleVersion.Core.svg?logo=nuget)][NugetPre]
[![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/Kieranties/SimpleVersion/1/master.svg?logo=azuredevops)][AzureRelease]
[![License](https://img.shields.io/github/license/Kieranties/SimpleVersion.svg?logo=github)][License]

SimpleVersion aims to simplify how you version the artifacts of your git repository.

Usage
-----
Check out the [documentation site][docs] for guidance.

Build
-----

To build SimpleVersion locally, run `build.ps1` from the root of the repository:
```posh
> .\build.ps1                       # => Runs a full build with unit tests
> .\build.ps1 -BuildDocs            # => Runs a full build and creates the docs site
> .\build.ps1 -BuildDocs -ServeDocs # Runs a full builds and serves the docs site
```

Contributions
-------------

Contributions, pull-requests, issues, and any other communications on the project
are most welcome!  Please use one of the [issue templates] to get going.

[NugetRel]:         https://www.nuget.org/packages?q=simpleversion&prerel=false
[NugetPre]:         https://www.nuget.org/packages?q=simpleversion
[AzureRelease]:     https://dev.azure.com/Kieranties/SimpleVersion/_build?definitionId=1
[License]:          https://kieranties.mit-license.org/
[Docs]:             https://simpleversion.kieranties.com
[Issue Templates]:  https://github.com/Kieranties/SimpleVersion/issues/new/choose
