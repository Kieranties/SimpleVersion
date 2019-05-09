<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = 'Debug',
    [String]$Version = '1.0.0-local',
    [String]$RootPath = $PSScriptRoot,
    [String]$ArtifactsPath = (Join-Path $RootPath 'artifacts'),
    [String]$BuildPath = (Join-Path $RootPath 'build'),
    [String]$DocsPath = (Join-Path $RootPath 'docs'),
    [String]$PackagesPath = (Join-Path $RootPath 'packages'),
    [Switch]$BuildDocs,
    [Switch]$ServeDocs,
    [String]$DocfxVersion = '2.42.0'
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
$env:Configuration = $Configuration
$env:Version = $Version

if($ServeDocs) {
    $BuildDocs = $true
}

# Build/Pack
Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Ignore
dotnet build
$distArtifacts = Join-Path $ArtifactsPath 'dist'
dotnet pack --no-restore --no-build -o $distArtifacts

# Unit Test
$testArtifacts = Join-Path $ArtifactsPath 'tests'
Get-ChildItem 'test' -Filter '*.csproj' -Recurse |
    ForEach-Object {
        $testArgs = @(
            '--no-restore', '--no-build'
            '--collect', '"Code Coverage"',
            '-s', "$PSScriptRoot\shared\codecoverage.runsettings"
            '--logger', 'trx',
            '-r', $testArtifacts
        )
        dotnet test $_.Fullname @testArgs
    }

# docs
if($BuildDocs) {
    # Install docfx
    $docfxRoot = "$BuildPath\docfx.console\$DocFxVersion"
    $docfx = "$docfxRoot\tools\docfx.exe"
    if(!(Test-Path $docfx)) {
        $temp = (New-TemporaryFile).FullName + '.zip'
        Invoke-WebRequest "https://www.nuget.org/api/v2/package/docfx.console/$DocFxVersion" -OutFile $temp
        Expand-Archive $temp -DestinationPath $docfxRoot
        Remove-Item $temp
    }

    $docfxArgs = @()
    if($ServeDocs) {
        $docfxArgs += '-s'
    }
    . $docfx "$DocsPath\docfx.json" @docfxArgs
    Copy-Item "$DocsPath\obj\site" -Recurse -Destination (Join-Path $ArtifactsPath 'docs') -Container
}