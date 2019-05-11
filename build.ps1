<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = $env:Configuration,
    [String]$Version = $env:Version,
    [String]$RootPath = $PSScriptRoot,
    [String]$ArtifactsPath = (Join-Path $RootPath 'artifacts'),
    [String]$DocsPath = (Join-Path $RootPath 'docs'),
    [String]$PackagesPath = (Join-Path $RootPath 'packages'),
    [Switch]$NoBuild,
    [Switch]$BuildDocs,
    [Switch]$ServeDocs,
    [String]$DocfxVersion = '2.42.0'
)

function exec([string]$cmd) {
    $currentPref = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    & $cmd @args
    $ErrorActionPreference = $currentPref
    if($LASTEXITCODE -ne 0) {
        Write-Error "[Errorcode $LASTEXITCODE] Command $cmd $args"
        exit $LASTEXITCODE
    }
}

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
if(!$Configuration) {
    $Configuration = 'Debug'
    $env:Configuration = $Configuration
}
if(!$Version) {
    $Version = '1.0.0-local'
    $env:Version = $Version
}
if($ServeDocs) {
    $BuildDocs = $true
}

# Build/Pack
Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Ignore
if(!$NoBuild) {
    exec dotnet build
    $distArtifacts = Join-Path $ArtifactsPath 'dist'
    exec dotnet pack --no-restore --no-build -o $distArtifacts

    # Unit Test
    $testArtifacts = Join-Path $ArtifactsPath 'tests'
    Get-ChildItem 'test' -Filter '*.csproj' -Recurse |
        ForEach-Object {
            $testArgs = @(
                '--no-restore', '--no-build'
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
