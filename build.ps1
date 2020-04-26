<#
    Local build script to perform build and test validation
#>
param(
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = 'Debug',
    [String]$RootPath = $PSScriptRoot,
    [String]$ArtifactsPath = (Join-Path $RootPath 'artifacts'),
    [String]$DocsPath = (Join-Path $RootPath 'docs'),
    [String]$PackagesPath = (Join-Path $RootPath 'packages'),
    [Switch]$NoBuild,
    [Switch]$BuildDocs,
    [Switch]$ServeDocs,
    [Switch]$Resources,
    [String]$DocfxVersion = '2.42.0'
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1

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

# Resources
if($Resources){
    exec dotnet msbuild /t:ResourceGen
    return
}

# Build/Test/Pack
if(!$NoBuild) {
    # Clean
    Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Ignore

    # Version
    dotnet tool restore
    $versionDetails = exec dotnet simpleversion
    $version = ($versionDetails | ConvertFrom-Json).Formats.Semver2
    if($env:TF_BUILD) { Write-Output "##vso[build.updatebuildnumber]$version" }
    
    $dotnetArgs = @('--configuration', $Configuration, "/p:Version=$version")

    # Pack
    $distArtifacts = Join-Path $ArtifactsPath 'dist'
    exec dotnet pack $dotnetArgs --output $distArtifacts

    # Unit Test
    $testArtifacts = Join-Path $ArtifactsPath 'tests'
    $testArgs = @(
        '--logger', 'trx'
        '-r', $testArtifacts
        "/p:MergeWith=$testArtifacts\coverage.json"
        '/p:CoverletOutputFormat=\"cobertura,json\"', "/p:CoverletOutput=$testArtifacts\"
    )
    exec dotnet test ($dotnetArgs + $testArgs)

    exec dotnet reportgenerator "-reports:$(Join-Path $testArtifacts '**/*.cobertura.xml')" "-targetDir:$(Join-Path $testArtifacts 'CoverageReport')"
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
