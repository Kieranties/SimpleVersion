Results
=======

How you handle the results of SimpleVersion will vary based on how you are
invoking the tool.  However, all implementations return the same results object.

The following is an example from invoking the command line tool:

```json
{
  "Version": "0.1.0",
  "Major": 0,
  "Minor": 1,
  "Patch": 0,
  "Revision": 0,
  "Height": 18,
  "HeightPadded": "0018",
  "Sha": "ebc8f22ae83bfa3c1e36d6bf70c2a383ae30c9dd",
  "BranchName": "preview/test",
  "Formats": {
    "Semver1": "0.1.0-alpha2-0018",
    "Semver2": "0.1.0-alpha2.18"
  }
}
```

Properties
----------

| Name | Type Value | Details |
| --- | --- | --- |
| Version | _int_._int_._int_(._int_) | The Major.Minor.Patch version value. If Revision was provided in the original format, it will also be included. |
| Major | int | The Major value reported in the Version string |
| Minor | int | The Minor value reported in the Version string |
| Patch | int | The Patch value reported in the Version string |
| Revision | int | The Revision value. Always 0 even if not included in the Version string |
| Height | int | The calculated height |
| HeightPadded | int | The cacluated height padded to four digits |
| Sha | string | The sha of the current commit at the time of invocation |
| BranchName | string | The checked out branch at the time of invocation |
| CanonicalBranchName | string | The full canonical name of the checked out branch at the time of invocation |

### Formats

Formats are specified combinations of the result values that follow a
specification. Currently only built in formats are supported, however custom
formats will be supported in a future version.

| Name | Type Value | Details |
| --- | --- | --- |
| Semver1 | string | The full version string following the [Semver1] spec |
| Semver2 | string | The full version string following the [Semver2] spec |

[Semver1]: https://semver.org/spec/v1.0.0.html
[Semver2]: https://semver.org/spec/v2.0.0.html