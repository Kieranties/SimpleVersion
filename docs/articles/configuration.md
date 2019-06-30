Configuration
=============

SimpleVersion reads the `.simpleversion.json` from the root of your git repo.
This file provides various configuration options.

Version
-------

The `Version` property allows you to specify the base version to be generated.
You  may set the property with a version string that consists of one to four
dot separated digits.

All of the following are accepted values:
```json
"Version" : "1"
"Version" : "1.2"
"Version" : "1.2.3"
"Version" : "1.2.3.4"
```

Label
-----

The `Label` property specifies an array of labels to be included in the version.

```json
"Label" : []
"Label" : ["alpha1"]
"Label" : ["alpha1", "test"]
```

> [!NOTE]
> By specifying values in the label, the version will be returned as a pre-release version.

Metadata
--------

The `Metadata` property specifies an array of values to be included as metadata
in the final version.

```json
"Metadata" : []
"Metadata" : ["demo"]
"Metadata" : ["demo", "sprint1"]
```

> [!WARNING]
> Currently, only `Semver2` format supports `metadata`.

Offset
------

Sometimes you may need to adjust the base value of the height. This could be
when migrating from a previous versioning pattern, if a number of commits
should be discounted, or any other reason.

Specify the `Offset` as a numeric value to impact the base value of the height.

```json
"Offset" : -5
"Offset" : 4
```

Branches
--------

The `Branches` section allows for branch specific rules and configuration to
be applied based on the branch currently being built.

### Release

`Release` allows for a list of regular expressions to be defined where each
may match to the current branch being built. If the current branch does not match
any of the expressions it will have the short sha of the current commit added
to the `Labels` property prefixed with `c` (for commit).

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

In the above example, any branch called _master_, starting with _preview/_ or
starting with _release/_ will **not** have the short sha appended. Generating a
Semver2 version of `0.1.0-alpha2.5` when there are five commits.

All other branches will append the short sha, generating a Semver2 version of
`0.1.0-alpha2.5.c903782` when there are five commits and the sha begins with
_903782_

> [!NOTE]
> Release branch configuration provides a simple way to identify what may be
> publicly shipped. If the version has a label containing the sha, you probably
> don't want it released.  You can enable all branches to be release branches
> using teh regular expression `.*`

### Overrides

Overrides allow for certain elements of the version to be reconfigured based
on the branch being built.  Overrides are matched by a regular expression where
only the first match (if found) is used.

```json
{
  "version": "0.2.0",
  "label": [ "alpha1" ],
  "branches": {
    "release": [
      "^refs/heads/master$",
      "^refs/heads/preview/.+$",
      "^refs/heads/release/.+$"
    ],
    "overrides": [
      {
        "match": "^refs/heads/feature/.+$",
        "metadata": [ "{shortbranchname}" ]
      },
      {
        "match": "^refs/heads/release/.+$",
        "label": [],
        "metadata": [ "*" ]
      }
    ]
  }
}
```

In the above example, any branch starting with _feature/_ will add the branches
shortname as metadata to the generated version. E.g. _feature/testing_ will
create a version of `0.2.0-alpha1.5.c903782+featuretesting` when there are five
commits and the sha begins with _903782_

Additionally, any branch beginning with _release/_ will strip the release label
and have the height added into the metadata.

> [!WARNING]
> Overrides will allow the same commit to be built with different versions
> depending on the current branch.

#### Override Properties

Override configuration has access to the following properties

| Key               | Type                  | Required | Description                                                                               |
| ----------------- | --------------------- | -------- | ----------------------------------------------------------------------------------------- |
| `match`           | string                | true     | Branches with a canonical branch name matching this regular expression will be overridden |
| `label`           | string array          | false    | Overrides `label` from the base configuration                                             |
| `prefixlabel`     | string array          | false    | Prefixes the base configuration `label` with the given values                             |
| `postfixlabel`    | string array          | false    | Postfixes the base configuration `label` with the given values                            |
| `insertlabel`     | int/string dictionary | false    | Inserts the given values into the base `label` at the index specified                     |
| `metadata`        | string array          | false    | Overrides `metdata` from the base configuration                                           |
| `prefixmetadata`  | string array          | false    | Prefixes the base configuration `metadata` with the given values                          |
| `postfixmetadata` | string array          | false    | Postfixes the base configuration `metadata` with the given values                         |
| `insertmetadata`  | int/string dictionary | false    | Inserts the given values into the base `metadata` at the index specified                  |


Replacement Tokens
------------------

SimpleVersion allows specific _tokens_ to be used in some properties to allow
substitution of values during invocation.  The following tokens may be used:


| Name               | Token                | Where                          | Description                                                                |
| ------------------ | -------------------- | ------------------------------ | -------------------------------------------------------------------------- |
| Height             | `*`                  | `version`, `label`, `metadata` | Inserts the calculated height                                              |
| Branch Name        | `{branchname}`       | `label`, `metadata`            | Inserts the canonical branch name, stripped of non-alphanumeric characters |
| Short Branch Name  | `{shortbranchname}`  | `label`, `metadata`            | Inserts the friendly branch name, stripped of non-alphanumeric characters  |
| Branch Name Suffix | `{branchnamesuffix}` | `label`, `metadata`            | Inserts the last segment of the canonical name of a branch                 |
| Short Sha          | `{shortsha}`         | `label`, `metadata`            | Inserts the first seven characters of the commit sha, prefixed with `c`    |
| Pull-Request Id    | `{pr}`               | `label`, `metadata`            | Inserts the id of the pull-request (or 0 by default)                       |
