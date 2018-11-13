param(
	[String]$srcPath = (Resolve-Path "$PSScriptroot\src"),
	[String]$artifactsPath = (New-Item "$PSScriptRoot\artifacts" -ItemType Container -Force | % FullName)
)

#TODO: Ensure dotnet

Write-Host $artifactsPath

#TODO: Run tests

dotnet publish "$srcPath\SimpleVersion.Command\SimpleVersion.Command.csproj" --output $artifactsPath