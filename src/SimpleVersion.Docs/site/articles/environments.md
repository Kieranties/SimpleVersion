Environments
============

SimpleVersion attempts to identify git information from the local environment.
When running on Continuous Integration (CI) systems you may need to prepare the
environment to cater for the CI systems specific approach to builds.

Azure Devops
------------

Azure Devops is identified as the build environment when the environment variable `TF_BUILD` is set.

When identified, the canonical and friendly branch names are derived from the `BUILD_SOURCEBRANCH` environment variable.

Fallback Environment
--------------------

For all environments, the build branch can be overridden by setting `simpleversion.sourcebranch` to the _full canonical branch name_.  When this variable is set it will override the resolution from any CI system as well as the local repository branch name.

### TeamCity

When building with TeamCity you can lift the branch name of the build to an environment variable by doing the following:

1. Open your build configuration, and select Parameters
1. Add a new paramter called `env.simpleversion.sourcebranch`
1. Set the value to `%teamcity.build.vcs.branch.{vcsid}%` where `{vcsid}` is the VCS Root of your repo
