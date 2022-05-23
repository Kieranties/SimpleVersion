#Requires -Version 7.0
using namespace System.IO

<#
    Local build script to perform acceptance testing
#>
param(
    [String]$ArtifactsPath = (Join-Path $PSScriptRoot '.artifacts'),
    [String]$Tag = '3.1-alpine3.14',
    [String]$OS = 'alpine',
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

$testOutput = Join-Path $ArtifactsPath 'tests'
New-Item $testOutput -ItemType Directory -Force > $null

$acceptanceRoot = [Path]::Combine($PSScriptRoot, 'test', 'acceptance')
$acceptanceDocker = Join-Path $acceptanceRoot 'Dockerfile'
$Targets | ForEach-Object {
    $tag = $_
    $buildTag = "simpleversion-acceptance:${version}-${tag}"
    $dockerBuildArgs = @(
        'build'
        '--build-arg', "SIMPLEVERSION=${version}"
        '--build-arg', "TAG=${tag}",
        '--build-arg', "OS=${OS}"
        '--tag', $buildTag
        '-f', $acceptanceDocker
        $distPath
    )
    if($ForceBuild) {
        $dockerBuildArgs += '--no-cache'
    }
    docker $dockerBuildArgs
    docker run -v "${acceptanceRoot}:/tests" $buildTag
    Move-Item (Join-Path $acceptanceRoot 'testResults.xml') (Join-Path $testOutput "${tag}.xml") -Force
}