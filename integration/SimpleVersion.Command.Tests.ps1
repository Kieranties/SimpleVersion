param(
	[String]$ArtifactsDir = "$PSScriptRoot\..\artifacts"
)

. $PSScriptRoot\utils.ps1

# Install SimpleVersion.Command
$command = NugetInstall SimpleVersion.Command $ArtifactsDir -PreRelease -Force
$env:Path += ";$command\tools"

function Invoke{
    $result = SimpleVersion $Args *>&1
    return [PSCustomObject]@{
        Output = $result
        ExitCode = $LASTEXITCODE 
    }
}

Describe 'SimpleVersion.Command'{
	Context 'Working Directory' {
		Context 'Not a git repo' {
			BeforeAll { Push-Location $env:TEMP }
			AfterAll { Pop-Location }

			It 'Throws error if repository cannot be found'{

				$expectedError = "[Error] Could not find git repository at '$pwd' or any parent directory"
				$result = Invoke

				$result.Output | Should -Be $expectedError
				$result.ExitCode | Should -Be -1
			}
		}
	}    
}