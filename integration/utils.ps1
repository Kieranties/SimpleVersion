param(
	[String]$temp = (Join-Path $PSScriptRoot 'temp')
)

if(!(Test-Path $temp)){
	New-Item $temp -ItemType Directory | Out-Null
}

# Install nuget if required
$nuget = Get-Command nuget -ErrorAction Ignore;
if(!$nuget){
	$nuget = Join-Path $temp nuget.exe
	$env:Path += ";$temp"
	Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nuget
}

function NugetInstall {
	param(		
		[String]$PackageName,		
		[String]$Source,
		[String]$Version,
		[Switch]$PreRelease,
		[Switch]$Force
	)

	if($Force){
		Get-ChildItem $temp -Filter "$PackageName.*" | Remove-Item -Recurse
	}

	$nugetArgs = @(
		'install', $PackageName,
		'-Source', $Source,
		'-OutputDirectory', $temp
	)
	if($Version) {
		$nugetArgs += @('-Version', $Version)
	}
	if($PreRelease){
		$nugetArgs += '-Pre'
	}
	nuget $nugetArgs | Write-Host

	if($LASTEXITCODE -ne 0) {
		throw "Nuget failed to install SimpleVersion.Command"
	}

	Get-ChildItem $temp -Filter "$PackageName.*" | Select-Object -First 1 -ExpandProperty FullName
}