param(
	[String]$srcPath = (Resolve-Path "$PSScriptroot\src"),
	[String]$artifactsPath = (New-Item "$PSScriptRoot\artifacts" -ItemType Container -Force | % FullName)
)

# Clean artifacts
Get-ChildItem $artifactsPath | Remove-Item -Recurse

# Publish self-contained app
dotnet publish "$srcPath\SimpleVersion.Command\SimpleVersion.Command.csproj" --output $artifactsPath