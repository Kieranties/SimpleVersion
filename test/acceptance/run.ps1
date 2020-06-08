# Local boostrap script to ensure clean pester environment
Remove-Module Pester # Clear out previous config/operator additions
Import-Module Pester
$configuration = [PesterConfiguration]::Default
$configuration.Run.Path = $PSScriptRoot
$configuration.CodeCoverage.Enabled = $false
$configuration.Output.Verbosity = 'Detailed'
Invoke-Pester -Configuration $configuration