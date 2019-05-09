SimpleVersion
=============
[![Nuget](https://img.shields.io/nuget/v/SimpleVersion.Core.svg?logo=nuget)][NugetRel]
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/SimpleVersion.Core.svg?logo=nuget)][NugetPre]
[![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/Kieranties/SimpleVersion/1/master.svg?logo=azuredevops)][AzureRelease]
[![License](https://img.shields.io/github/license/Kieranties/SimpleVersion.svg?logo=github)][License]

Brief
-----

SimpleVersion aims to simplify how you version the artifacts of your git repository.

Using SimpleVersion you can generate consistent, expected version numbers for
each commit in your repository, based on a few simple configuration options.

SimpleVersion uses some base configuration and the 'height' of your commit history
to generate a version.  Height is calculated as the number of commits _since the
last change to the .simpleversion.json file_.

> [!IMPORTANT]
> You must commit changes to the file for SimpleVersion to identify the change.

Future versions of SimpleVersion aim to provide:
+ Validation of the version based on branch configuration
+ Custom formatting configuration to support custom versioning schemes
+ Override logic with custom token replacement syntax
+ [and more!][FeatureBacklog]

Why?
----

SimpleVersion borrows ideas from other excellent versioning tools, notably:
[GitVersion] and [NerdBank.GitVersioning][NerdBank]. These tools also deliver
the ability to version your repository based on the commits but work in a
different manner:
+ GitVersion relies heavily on branch names, merge messages and more. I have found this
can sometimes cause issues without extreme management of best practices.
+ Nerdbank is an excellent tool that removes many of the issues I ran into with
GitVersion, however is more limited in scope for custom labelling formats.

Usage
-----

To use SimpleVersion, you simply need to add a `.simpleversion.json` file to the
root of your git repository and commit it.

## Configuration

```json
{
  "version": "0.1.0",
  "label": [ "alpha2" ],
  "branches": {
    "release": [
      "^refs/heads/master$",
      "^refs/heads/preview/.+$",
      "^refs/heads/release/.+$"
    ]
  }
}
```
The above configuration tells SimpleVersion that the version will be `0.1.0` and
should have a release label of `alpha2`.  As there is a pre-release label, the
height will be appended to label, generating a [Semver2] version `0.1.0-alpha2.3`
if there were three commits since the version was last set.

For further guidance, see the [configuration documentation][ConfigDoc].

Resetting The Height
--------------------

The height will be reset to 0 when SimpleVersion detects a change to either the
`version` or the `label` in the `.simpleversion.json` file.

[semver2]:        https://semver.org/spec/v2.0.0.html
[GitVersion]:     https://github.com/GitTools/GitVersion
[NerdBank]:       https://github.com/aarnott/Nerdbank.GitVersioning
[ConfigDoc]:      /articles/configuration.html
[NugetRel]:       https://www.nuget.org/packages?q=simpleversion&prerel=false
[NugetPre]:       https://www.nuget.org/packages?q=simpleversion
[AzureRelease]:   https://dev.azure.com/Kieranties/SimpleVersion/_build?definitionId=1
[License]:        https://kieranties.mit-license.org/
[FeatureBacklog]: https://github.com/kieranties/simpleversion/issues?q=is%3Aissue+is%3Aopen+label%3A%22%3Asparkles%3A+feature%22
