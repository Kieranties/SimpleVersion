#Requires -Version 7.0
using namespace System.IO

<#
    Local build script to perform acceptance testing
#>
param(
    [String]$ArtifactsPath = (Join-Path $PSScriptRoot '.artifacts'),
    [Switch]$ForceBuild
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
$tag = "simpleversion-acceptance:${version}"
$dockerBuildArgs = @(
    'build'
    '--build-arg', "SIMPLEVERSION_VERSION=${version}"
    '--tag', $tag
    '-f', $acceptanceDocker
    $distPath
)
if($ForceBuild) {
    $dockerBuildArgs += '--no-cache'
}
docker $dockerBuildArgs
docker run -v "${acceptanceRoot}:/tests" $tag
$testOutput = Join-Path $ArtifactsPath 'tests'
New-Item $testOutput -ItemType Directory -Force > $null
Copy-Item (Join-Path $acceptanceRoot 'testResults.xml') (Join-Path $testOutput 'acceptanceResults.xml')