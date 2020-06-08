#Requires -Version 7.0
using namespace System.IO

<#
    Local build script to perform acceptance testing
#>
param(
    [String]$ArtifactsPath = (Join-Path $PSScriptRoot '.artifacts')
)

$ErrorActionPreference = 'Stop'

$distPath = Join-Path $ArtifactsPath 'dist'
$version = Get-ChildItem $distPath -Filter 'SimpleVersion.Tool.*' |
    Select-Object -ExpandProperty BaseName |
    ForEach-Object { $_ -replace 'SimpleVersion.Tool.',''}
if(-not $version) {
    throw "No dist output for to test"
}

$acceptanceRoot = [Path]::Combine($PSScriptRoot, 'test', 'acceptance')
$acceptanceDocker = Join-Path $acceptanceRoot 'Dockerfile'
docker build --build-arg "SIMPLEVERSION_VERSION=${version}" --tag simpleversion-acceptance -f $acceptanceDocker $distPath
docker run -v "${acceptanceRoot}:/tests" simpleversion-acceptance
Copy-Item (Join-Path $acceptanceRoot 'testResults.xml') ([Path]::Combine($ArtifactsPath, 'test', 'acceptanceResults.xml'))