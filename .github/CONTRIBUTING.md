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

To ensure dependencies are kept in sync across the projects, the `Microsoft.Build.CentralPackageVersions` SDK is used to enable versions to be
tracked in a single location.

When adding a reference to a _new_ package, first add the `PackageReference` to
`Packages.props` with the specified version to be used.

> Note: Use `Update` instead of include in the reference.

Then add the `PackageReference` to the `*.csproj` where the package will be used.

> Note: Do not include the `Version` attribute in the `*.csproj`

### String Resources

This project utilizes a custom process to generate classes to manage string localization.
To localize strings within a library:

1. Create a resx file with _no culture extension_ (e.g. `Resources.rex` **not** `Resources.en.resx`)
2. In the relevant csproj _remove_ the auto added designer class
3. In the relevant csproj _replace_ the metadata for the resx file with `<UseResourceGen>true</UseResourceGen>`
4. Run `build.ps1 -Resources` from the root of the repository.

> Additional metadata tokens may be set (the defaults should be sufficient) See `Directory.Build.targets` for details
When ever you make a change to your resource file, re-run `build.ps1 -Resources`.

#### Providing Parameter Values

The resource generator will create properties for resource strings by default.
If the string contains token replacements (e.g. `My {0} {1} string`) provide a csv comment for the entry
with the names of the expected members (e.g. `first,second`)

#### Adding Additional Cultures

Additional cultures can be added by providing culture specific resource files (e.g. `Resources.en.resx`, `Resources.en-gb.resx`)
These files need no special treatment and only need to be added to the project.

#### Validation

When the class files are generated they hold a reference to the hash of the source resource file.
When the project is built the hashes are compared.  If a difference is found errors are thrown.

The issues can be resolved by regenerating the resource.

Workflow with Git
-----------------

### Commit Messages

This project follows conventional commit practices.  This aims to provide clear
and concise information for each commit being added to the repository.

> For more information see: https://www.conventionalcommits.org

When writing a commit message, the message should be written in the format:
```
<type>[optional scope]: <description>

[optional body]

[optional footer]
```

The `<type>` should be *one* of the following:

+ `fix` - Resolving a bug
+ `feat` (or `feature`) - Implementing a new feature
+ `refactor` - Refactoring to improve the code base
+ `docs` - Adding documentation
+ `chore` - Trivial/Arbitrary task
+ `test` - Adding tests
+ `improvement` - Improving the implementation of existing code.

The `<description>` should briefly describe the changes being addressed.

The optional `scope` can be added to allow greater clarity on the change being
committed, e.g.: `fix(config): Return co

Additional information should be added in the optional `body`. Any issues that
are resolved in the commit should be set flagged using `Fixes #<issueid>`

The footer should be used to identify breaking changes.  Any commit may contain
a breaking change.  It should be identified with `BREAKING CHANGE: <description>`

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

Pull-requests are squash-merged into the master branch, so if you have not
precisely followed the conventional commits guidelines, don't worry.  The
approver will clean things up.


Contact
-------

If you have any queries, just raise a [question].

[question]: https://github.com/Kieranties/SimpleVersion/issues/new/choose
