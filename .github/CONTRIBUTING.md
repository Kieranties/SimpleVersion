How to Contribute
=================

Thinking of contributing to this project? Great!

Before you get started, please be aware of the following information.  Following
the guidance in this document will help to get your features and bug fixes merged
in quickly and with minimal fuss.

> Don't worry!  This is only guidance and aims to help not hinder your contribution.
> The project owners will guide you where needed.

Code Formatting
---------------

All code should be implemented in a consistent manner, with formatting following
the conventions of existing code.  To ensure the formatting standards are met this
project uses .editorconfig.  All good IDEs will identify this file and provide hints
when the standards are not met.

The standards are loosely applied when building using the Debug configuration
and strictly applied when built for Release.

Before you complete a pull-request, make sure you have no warnings or errors,
otherwise your pull-request build **will** fail.

Code Quality
------------

### Test Coverage

All code changes require tests to cover the changes.  This could be updating
existing tests are adding new tests for new features.  See the test projects
for guidance on writing tests.

### Package References

To ensure dependendencies are kept in sync across the projects, the `Microsoft.Build.CentralPackageVersions` SDK is used to enable versions to be
tracked in a single location. When creating a new project, add `<Sdk Name="Microsoft.Build.CentralPackageVersions" />`
as the first node under the `<Project>`.

When adding a reference to a package, first add the `PackageReference` to
`Packages.props` with the specified version to be used.

> Note: Use `Update` instead of include in the reference.

Then add the `PackageReference` to the `*.csproj` where the package will be used.

> Note: Do not include the `Version` attribute in the `*.csproj`

Workflow with Git
-----------------

### Commit Messages

This project follows conventional commit practices.  This aims to provide clear
and concise information for each commit being added to the repo.

> For more information see: https://www.conventionalcommits.org

When writing a commit message, the message should be written in the format:
```
<type>[optional scope]: <description>

[optional body]

[optional footer]
```

The `\<type>` should be *one* of the following:

+ `fix` - Resolving a bug
+ `feat` (or `feature`) - Implementing a new feature
+ `refactor` - Refactoring to improve the code base
+ `docs` - Adding documentation
+ `chore` - Trivial/Arbitrary task
+ `test` - Adding tests
+ `improvement` - Improving the implementation of existing code.

The `\<description>` should briefly describe the changes being addressed.

The optional `scope` can be added to allow greater clarity on the change being
committed, e.g.: `fix(config): Return co

Additional information should be added in the optional `body`. Any issues that
are resolved in the commit should be set flagged using `Fixes #\<issueid>`

The footer should be used to identify breaking changes.  Any commit may contain
a breaking change.  It should be identified with `BREAKING CHANGE: \<description>`

#### Examples:
```
fix: Return correct count of items

Fixes #123
```

```
feat(result): Implement Semver3

Result object now has features to return semver3 calculations
Fixes #567

BREAKING CHANGE: Schema model changed for simplicity
```

### Pull-Requests

Once you have completed your changes, raise a pull-request to the `master`
branch.  An automated build will be kicked off and a code review request
will be raised to the repository authors.

Please complete the pull-request template with any additional information for
the reviewer.

Pull-Requests are squash-merged into the master branch, so if you have not
precisely followed the conventional commits guidelines, don't worry.  The
approver will clean things up.
