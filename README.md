SimpleVersion
=============
[![Nuget](https://img.shields.io/nuget/v/SimpleVersion.Core.svg?logo=nuget&color=blue)][NugetRel]
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SimpleVersion.Core.svg?logo=nuget)][NugetPre]
[![Build Status](https://dev.azure.com/Kieranties/SimpleVersion/_apis/build/status/Kieranties.SimpleVersion?branchName=master)][AzureRelease]
[![GitHub issues](https://img.shields.io/github/issues/Kieranties/Simpleversion.svg?logo=github)][Issues]
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

Issues
------

Raise and issue using one of the [templates] and it'll get automatically added to the [backlog].

Contributions
-------------

Thinking of implementing or fixing something? Great! Contributions and pull-requests are most welcome.

Please take a look at the [Contributing Guide] and [Code of Conduct] before submitting your pull request.

[NugetRel]:           https://www.nuget.org/packages?q=simpleversion&prerel=false
[NugetPre]:           https://www.nuget.org/packages?q=simpleversion
[AzureRelease]:       https://dev.azure.com/Kieranties/SimpleVersion/_build/latest?definitionId=1&branchName=master
[License]:            https://kieranties.mit-license.org/
[Issues]:             https://github.com/kieranties/simpleversion/issues
[Docs]:               https://simpleversion.kieranties.com
[templates]:          https://github.com/Kieranties/SimpleVersion/issues/new/choose
[Contributing Guide]: https://github.com/Kieranties/SimpleVersion/blob/master/.github/CONTRIBUTING.md
[Code of Conduct]:    https://github.com/Kieranties/SimpleVersion/blob/master/.github/CODE_OF_CONDUCT.md
[backlog]:            https://github.com/Kieranties/SimpleVersion/projects/3