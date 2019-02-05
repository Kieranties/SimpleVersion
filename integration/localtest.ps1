#! bin/pwsh

param(
	[String]$ArtifactsDir = "$PSScriptRoot\..\artifacts"
)

. $PSScriptRoot\utils.ps1

# Ensure we have our paths
if(!(Test-Path $ArtifactsDir)){
	throw "ArtifactsDir does not exist: $ArtifactsDir";
};

# Install Pester
$pesterPath = NugetInstall Pester https://www.powershellgallery.com/api/v2/
Import-Module "$pesterPath\Pester"

# Collect pester args
$pesterArgs = @{
	Script = $PSScriptRoot
	OutputFormat = "NunitXml"
	OutputFile = "$ArtifactsDir\IntegrationTests.Pester.xml"
	PassThru = $true
}

# Run Pester
$pesterResults = Invoke-Pester @pesterArgs
if($pesterResults.FailedCount -gt 0){
	throw "Integration tests failed: $FailedCount/$TotalCount"
}