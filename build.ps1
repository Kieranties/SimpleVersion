param(
	[String]$SrcPath = (Resolve-Path "$PSScriptroot\src"),
	[String]$ArtifactsPath = (New-Item "$PSScriptRoot\artifacts" -ItemType Container -Force | ForEach-Object FullName)
)


# Clean artifacts
Get-ChildItem $ArtifactsPath | Remove-Item -Recurse

# Publish self-contained app
dotnet publish "$SrcPath\SimpleVersion.Command\SimpleVersion.Command.csproj" --output $ArtifactsPath
