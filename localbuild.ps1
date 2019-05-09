<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Unit','Integration','Docs')]
    [string[]]$Tasks = @('Unit'),
    [String]$RootPath = $PSScriptRoot,
    [String]$ArtifactsPath = (Join-Path $RootPath 'artifacts'),
    [String]$BuildPath = (Join-Path $RootPath 'build'),
    [String]$DocsPath = (Join-Path $RootPath 'docs'),
    [Switch]$NoBuild,
    [Switch]$ServeDocs

)

$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1

# Clean and build
if(!$NoBuild) {
    Get-ChildItem $ArtifactsPath -ErrorAction Ignore | Remove-Item -Recurse -Force
    dotnet pack (Join-Path $RootPath 'SimpleVersion.sln') -o $ArtifactsPath
}

if($Tasks -contains 'Unit') {
    Get-ChildItem 'test' -Filter '*.csproj' -Recurse | ForEach-Object {
        dotnet test $_.Fullname
    }
}

if($Tasks -contains 'Integration'){
    . "$RootPath\integration\localtest.ps1" $ArtifactsPath
}

if($Tasks -contains 'Docs') {
    # Install docfx
    New-Item $BuildPath -ItemType Directory -Force | Out-Null
    $docfx = "$BuildPath\docfx.console*\tools\docfx.exe"
    if(!(Test-Path $docfx)) {
        Install-Package docfx.console -Destination $BuildPath -Source 'https://www.nuget.org/api/v2/' -ForceBootstrap -Force
    }
    $docfx = Resolve-Path $docfx
    $docsDest = Join-path $ArtifactsPath 'docs'
    Remove-Item $docsDest -Recurse -Force -ErrorAction Ignore

    Push-Location $DocsPath
    try {
        $docfxArgs = @()
        if($ServeDocs) {
            $docfxArgs += '-s'
        }
        . $docfx @docfxArgs
        Copy-Item '.\obj\site' -Recurse -Destination $docsDest -Container
    } finally {
        Pop-Location
    }
}
