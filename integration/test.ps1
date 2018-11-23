#! bin/pwsh

param(
	[String]$ArtifactsDir = "$PSScriptRoot\..\artifacts",
	[String]$WorkingDir = "$ArtifactsDir\integration"
)

$InformationPreference = 'Continue'

# Ensure we have our paths
if(!(Test-Path $ArtifactsDir)){
	throw "ArtifactsDir does not exist: $ArtifactsDir";
};

New-Item $WorkingDir -ItemType Directory -Force | Out-Null

# Install nuget if required
$nuget = Get-Command nuget -ErrorAction Ignore;
if($nuget){;
	$nuget = $nuget.Source
} else {
	$nuget = Join-Path $WorkingDir nuget.exe
	$env:Path += ";$WorkingDir"
	Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nuget
}

function NugetInstall {
	param(
		[Parameter(Mandatory)]
		[String]$PackageName,
		[Parameter(Mandatory)]
		[String]$Source,
		[String]$Version,
		[Switch]$PreRelease
	)
	$nugetArgs = @(
		'install', $PackageName,
		'-Source', $Source,
		'-OutputDirectory', $WorkingDir
	)
	if($Version) {
		$nugetArgs += @('-Version', $Version)
	}
	if($PreRelease){
		$nugetArgs += '-Pre'
	}
	nuget $nugetArgs | Write-Information

	if($LASTEXITCODE -ne 0) {
		throw "Nuget failed to install SimpleVersion.Command"
	}

	Get-ChildItem $WorkingDir -Filter "$PackageName.*" | Select-Object -First 1 -ExpandProperty FullName
}

# Install Pester
$pesterPath = NugetInstall Pester https://www.powershellgallery.com/api/v2/
Import-Module "$pesterPath\Pester"

# Install SimpleVersion.Command
$command = NugetInstall SimpleVersion.Command $ArtifactsDir -PreRelease
$env:Path += ";$command\tools"

$pesterArgs = @{
	Script = $PSScriptRoot
	OutputFormat = "NunitXml"
	OutputFile = "$ArtifactsDir\PesterTests.xml"
	PassThru = $true
}

$pesterResults = Invoke-Pester @pesterArgs
if($pesterResults.FailedCount -gt 0){
	#throw "Integration tests failed: $FailedCount/$TotalCount"
}