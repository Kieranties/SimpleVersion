[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [String]$ResourcePath,
    [Parameter(Mandatory)]
    [String]$GeneratedFileName
)

Import-Module $PSScriptRoot\ManageResources.psm1 -Force
Test-Resource $ResourcePath $GeneratedFileName
