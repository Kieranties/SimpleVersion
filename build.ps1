<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = 'Debug',
    [String]$Version = '1.0.0-local',
    [String]$RootPath = $PSScriptRoot,
    [String]$ArtifactsPath = (Join-Path $RootPath 'artifacts'),
    [String]$DocsPath = (Join-Path $RootPath 'docs'),
    [String]$PackagesPath = (Join-Path $RootPath 'packages'),
    [Switch]$NoBuild,
    [Switch]$BuildDocs,
    [Switch]$ServeDocs,
    [Switch]$Resources,
    [String]$DocfxVersion = '2.42.0',
    [string]$DotnetInstallScript = 'https://dot.net/v1/dotnet-install.ps1',
    [string]$DotnetLocalInstallScript = (Join-Path $PSScriptRoot 'dotnet-install.ps1')
)

# Ensure we have the latest LTS installed for the user before processing further
function CheckDotNet {
    if(-not(Test-Path $DotnetLocalInstallScript)){
        Invoke-WebRequest -Uri $DotnetInstallScript -OutFile $DotnetLocalInstallScript
    }
    . $DotnetLocalInstallScript
}

function exec([string]$cmd) {
    $currentPref = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    & $cmd @args
    $ErrorActionPreference = $currentPref
    if($LASTEXITCODE -ne 0) {
        Write-Error "[Error code $LASTEXITCODE] Command $cmd $args"
        exit $LASTEXITCODE
    }
}

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
if($ServeDocs) {
    $BuildDocs = $true
}

CheckDotNet

# Resources
if($Resources){
    exec dotnet msbuild /t:ResourceGen
    return
}

# Build/Pack
Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Ignore
if(!$NoBuild) {
    exec dotnet build --configuration $Configuration /p:Version=$Version
    $distArtifacts = Join-Path $ArtifactsPath 'dist'
    exec dotnet pack --no-build --no-restore --configuration $Configuration --output $distArtifacts

    # Unit Test
    $testArtifacts = Join-Path $ArtifactsPath 'tests'
    Get-ChildItem 'test' -Filter '*.csproj' -Recurse |
        ForEach-Object {
            $testArgs = @(
                '--no-restore', '--no-build'
                '--configuration', $Configuration
                '--logger', 'trx'
                '-r', $testArtifacts
                '/p:CollectCoverage=true', "/p:MergeWith=$testArtifacts\coverage.json"
                '/p:CoverletOutputFormat=\"cobertura,json\"', "/p:CoverletOutput=$testArtifacts\"
            )
            exec dotnet test $_.Fullname @testArgs
        }
}

# docs
if($BuildDocs) {
    # Install docfx
    $docfxRoot = "$PackagesPath\docfx.console\$DocFxVersion"
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
    Remove-Item "$DocsPath\obj" -Recurse -Force -ErrorAction Ignore
    exec $docfx "$DocsPath\docfx.json" @docfxArgs
    Copy-Item "$DocsPath\obj\site" -Recurse -Destination (Join-Path $ArtifactsPath 'docs') -Container
}
