param(
	[String]$SrcPath = (Resolve-Path "$PSScriptroot\src"),
	[String]$ArtifactsPath = (New-Item "$PSScriptRoot\artifacts" -ItemType Container -Force | ForEach-Object FullName),
	[String]$Version = "0.1.0",
	[String]$Label = "alpha1"
)

# Clean artifacts
Get-ChildItem $ArtifactsPath | Remove-Item -Recurse

# For now, we are just counting the number of commits on this branch if we have a label
# This will be modified in the future to use a subset of the functionality we are actually implementing

# Assumes git is on the path
$height = git rev-list --count HEAD
$fullLabel = if($Label){
	"-${Label}.${height}"
} else {
	"+${height}"
}

# Set version info
$env:Version = $Version
$env:VersionSuffix = $fullLabel

# if running on the build server we want the build version to be updated
if($env:TF_Build){
	Write-Output "##vso[build.updatebuildnumber]$Version$fullLabel"
}

# Publish self-contained app
dotnet publish "$SrcPath\SimpleVersion.Command\SimpleVersion.Command.csproj" --output $ArtifactsPath --configuration Release

# Package
$destination = "$ArtifactsPath\SimpleVersion.$Version$fullLabel.zip"
Compress-Archive -Path $ArtifactsPath\*.exe,$PSScriptroot\Readme.md -DestinationPath $destination