Configuration
=============

SimpleVersion reads the `.simpleversion.json` from the root of your git repo.
This file provides various configuration options.

Version
-------

The `Version` property allows you to specify the base version to be generated.
You  may set the property with a version string that consists of one to four
dot seperated digits.

All of the following are accepted values:
```json
"Version" : "1"
"Version" : "1.2"
"Version" : "1.2.3"
"Version" : "1.2.3.4"
```

> You may also specify '*' in place of a number to insert the generated height.

Label
-----

The `Label` property specifies an array of labels to be included in the version.

> By specifying values in the label, the version will be returned as a pre-release version.

```json
"Label" : []
"Label" : ["alpha1"]
"Label" : ["alpha1", "test"]
```
> You may also specify '*' in place of a value to insert the generated height.

MetaData
--------

The `MetaData` property specifies an array of values to be included as metadata
in the final version.

> Currently, only `Semver2` format supports `metadata`.

```json
"MetaData" : []
"MetaData" : ["demo"]
"MetaData" : ["demo", "sprint1"]
```

> You may also specify '*' in place of a value to insert the generated height.

OffSet
------

Sometimes you may need to adjust the base value of the height. This could be
when migrating from a previous versioning pattern, if a number of commits
should be discounted, or any other reason.

Specify the `OffSet` as a numeric value to impact the base value of the height.

```json
"OffSet" : -5
"OffSet" : 4
```

Branches
--------

The `Branches` section allows for branch specific rules and configuration to
be applied based on the branch currently being built.

### Release

`Release` allows for a list of regular expressions to be defined where each
may match to the current branch being built. If the current branch does not match
any of the expressions it will have the short sha of the current commit added
to the `Labels` property.

```json
{
  "version": "0.1.0",
  "label": [ "alpha2" ],
  "branches": {
    "release": [
      "^master$",
      "^preview/.+$",
      "^release/.+$"
    ]
  }
}
```

In the above example, any branch called _master_, starting with _preview/_ or
starting with _release/_ will **not** have the short sha appended. Generating a
Semver2 verison of `0.1.0-alpha2.5` when there are five commits.

All other branches will append the short sha, generating a Semver2 version of
`0.1.0-alpha2.5.903782` when there are five commits and the sha begins with
_903782_