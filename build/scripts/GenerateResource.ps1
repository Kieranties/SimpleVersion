[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [String]$ResourcePath,
    [Parameter(Mandatory)]
    [String]$GeneratedFileName,
    [Parameter(Mandatory)]
    [String]$Namespace,
    [Parameter(Mandatory)]
    [String]$ClassName,
    [Parameter(Mandatory)]
    [String]$AccessModifier
)

Import-Module $PSScriptRoot\ManageResources.psm1 -Force
Update-Resource $ResourcePath $GeneratedFileName $Namespace $ClassName $AccessModifier
