#Requires -Version 7.0
using namespace System.IO

<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = 'Debug',
    [String]$RootPath = $PSScriptRoot,
    [String]$ArtifactsPath = (Join-Path $RootPath '.artifacts'),
    [String]$DocsPath = (Join-Path $RootPath 'docs'),
    [String]$PackagesPath = (Join-Path $RootPath 'packages'),
    [Switch]$NoBuild,
    [Switch]$BuildDocs,
    [Switch]$ServeDocs,
    [Switch]$Resources,
    [String]$DocfxVersion = '2.42.0'
)

. ([Path]::Combine($PSScriptRoot, 'build', 'scripts', 'Utils.ps1'))

$ErrorActionPreference = 'Stop'

# Resources
if($Resources){
    exec dotnet msbuild /t:ResourceGen
    return
}

# Build/Test/Pack
if(!$NoBuild) {
    
    # Clean
    Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Ignore

    # Restore Tools
    dotnet tool restore

    # Version
    $versionDetails = exec dotnet simpleversion
    $version = ($versionDetails | ConvertFrom-Json).Formats.Semver2
    if($env:TF_BUILD) { Write-Output "##vso[build.updatebuildnumber]$version" }
    
    # Default Args
    $dotnetArgs = @('--configuration', $Configuration, "/p:Version=$version")

    # Build
    exec dotnet build $dotnetArgs

    # Test
    $testArtifacts = Join-Path $ArtifactsPath 'tests'
    exec dotnet test $dotnetArgs -r $testArtifacts --no-build
    exec dotnet reportgenerator "-reports:$(Join-Path $testArtifacts '**/*.cobertura.xml')" "-targetDir:$(Join-Path $testArtifacts 'CoverageReport')" "-reportTypes:HtmlInline_AzurePipelines_Dark"

    # Pack
    $distArtifacts = Join-Path $ArtifactsPath 'dist'
    exec dotnet pack $dotnetArgs --output $distArtifacts --no-build
}

# docs
if($BuildDocs -or $ServeDocs) {
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
