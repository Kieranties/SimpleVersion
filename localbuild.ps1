<#
	Local build script to perform build and test validation
#>
param(
	[String]$Root = $PSScriptRoot,
	[String]$Artifacts = (Join-Path $Root 'artifacts'),
	[Switch]$NoIntegration,
	[Switch]$NoUnit
)

$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1

# Clean previous run
if(Test-Path $Artifacts){
	Get-ChildItem $Artifacts -Filter '*.nupkg' | Remove-Item
}

# Pack
dotnet pack (Join-Path $Root 'SimpleVersion.sln') -o $Artifacts

# Unit
if(!$NoUnit){
	Get-ChildItem 'test' -Filter '*.csproj' -Recurse | ForEach-Object {
		dotnet test $_.Fullname
	}
}

# Integration
if(!$NoIntegration){
	$integration = [System.IO.Path]::Combine($Root, 'integration', 'localtest.ps1')
	. $integration $Artifacts
}