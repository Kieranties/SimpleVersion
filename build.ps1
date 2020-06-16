#Requires -Version 7.0
using namespace System.IO

<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = 'Debug',
    [String]$ArtifactsPath = (Join-Path $PSScriptRoot '.artifacts'),
    [Switch]$ServeDocs = $false,
    [Switch]$Resources
)

. ([Path]::Combine($PSScriptRoot, 'build', 'scripts', 'Utils.ps1'))
$docsProject = [Path]::Combine($PSScriptRoot, 'src', 'SimpleVersion.Docs')

$ErrorActionPreference = 'Stop'

# Resources
if ($Resources) {
    exec dotnet msbuild /t:ResourceGen
    return
}

# Clean
Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Ignore

# Restore Tools
dotnet tool restore

# Version
$versionDetails = exec dotnet simpleversion
$version = ($versionDetails | ConvertFrom-Json).Formats.Semver2
if ($env:TF_BUILD) { Write-Output "##vso[build.updatebuildnumber]$version" }

# Default Args
$dotnetArgs = @('--configuration', $Configuration, "/p:Version=$version")

# Build
exec dotnet build $dotnetArgs

# Docs
exec dotnet publish $docsProject $dotnetArgs -o "${ArtifactsPath}/docs" /p:ServeDocs=$ServeDocs
if ($ServeDocs) {
    return
}

# Test
$testArtifacts = Join-Path $ArtifactsPath 'tests'
exec dotnet test $dotnetArgs --results-directory $testArtifacts --no-build
exec dotnet reportgenerator "-reports:$(Join-Path $testArtifacts '**/*.cobertura.xml')" "-targetDir:$(Join-Path $testArtifacts 'CoverageReport')" "-reporttypes:HtmlInline_AzurePipelines"

# Pack
$distArtifacts = Join-Path $ArtifactsPath 'dist'
exec dotnet pack $dotnetArgs --output $distArtifacts --no-build