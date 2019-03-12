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

function Validate {
	param(
		[PSCustomObject]$Result,
		[ScriptBlock]$AsError = $null,
		[ScriptBlock]$AsSuccess = $null
	)

	if($AsError) {
		$Result.ExitCode | Should -Be -1
		$errorMessage =  $Result.Output
		& $AsError $errorMessage
	} else {
		$Result.ExitCode | Should -Be 0
		$json = ConvertFrom-Json ($Result.Output -join '')
		& $AsSuccess $json
	}
}

Describe 'SimpleVersion.Command'{

	Context 'Working Directory' {
		Context 'Not a git repo' {
			BeforeAll { Push-Location $Testdrive }
			AfterAll { Pop-Location }

			It 'Throws error if repository cannot be found'{

				$result = Invoke

				Validate $result -AsError {
					$errorMEssage | Should -Be "[Error] Could not find git repository at '$pwd' or any parent directory"
				}
			}
		}

		Context 'Existing Git Repo'{
			BeforeAll {
				$dir = New-Item "${TestDrive}\$(Get-Random)" -ItemType Directory
				Push-Location $dir
				git init
				git config user.email "SimpleVersion@Kieranties.com"
				git config user.name "Simple Version"
			}

			AfterAll {
				Pop-Location
				Remove-Item $dir -Recurse -Force
			}

			It 'Throws error if no commits for version file'{

				$result = Invoke

				Validate $result -AsError {
					$errorMEssage | Should -Be "[Error] No commits found for '.simpleversion.json'"
				}
			}

			It 'Returns base values for initial commit' {
				Copy-Item $PSScriptRoot\assets\.simpleversion.json -Destination $pwd
				git add *
				git commit -m 'Initial commit'

				$expectedSha = git rev-parse HEAD
				$result = Invoke

				Validate $result -AsSuccess {
					$json.Version | Should -Be '0.1.0'
					$json.Major | Should -Be 0
					$json.Minor | Should -Be 1
					$json.Patch | Should -Be 0
					$json.Revision | Should -Be 0
					$json.Height | Should -Be 1
					$json.HeightPadded | Should -Be '0001'
					$json.Sha | Should -Be $expectedSha
					$json.BranchName | Should -Be 'master'
					$json.Formats.Semver1 | Should -Be '0.1.0-alpha1-0001'
					$json.Formats.Semver2 | Should -Be '0.1.0-alpha1.1'
				}
			}

			It 'Increments for each commit' {
				Copy-Item $PSScriptRoot\assets\.simpleversion.json -Destination $pwd
				git add *
				git commit -m 'Initial commit'
				git commit -m 'empty' --allow-empty
				git commit -m 'empty' --allow-empty
				git commit -m 'empty' --allow-empty

				$expectedSha = git rev-parse HEAD
				$result = Invoke
				Validate $result -AsSuccess {
					$json.Version | Should -Be '0.1.0'
					$json.Major | Should -Be 0
					$json.Minor | Should -Be 1
					$json.Patch | Should -Be 0
					$json.Revision | Should -Be 0
					$json.Height | Should -Be 4
					$json.HeightPadded | Should -Be '0004'
					$json.Sha | Should -Be $expectedSha
					$json.BranchName | Should -Be 'master'
					$json.Formats.Semver1 | Should -Be '0.1.0-alpha1-0004'
					$json.Formats.Semver2 | Should -Be '0.1.0-alpha1.4'
				}
			}

			It 'Resets on version change' {
				Copy-Item $PSScriptRoot\assets\.simpleversion.json -Destination $pwd
				git add *
				git commit -m 'Initial commit'
				git commit -m 'empty' --allow-empty
				git commit -m 'empty' --allow-empty
				$json = Get-Content $pwd\.simpleversion.json -Raw | ConvertFrom-Json
				$json.Version = "1.0.0"
				Set-Content $pwd\.simpleversion.json (ConvertTo-Json $json -Depth 100)
				git add *
				git commit -m 'Updated version'
				git commit -m 'empty' --allow-empty

				$expectedSha = git rev-parse HEAD
				$result = Invoke

				Validate $result -AsSuccess {
					$json.Version | Should -Be '1.0.0'
					$json.Major | Should -Be 1
					$json.Minor | Should -Be 0
					$json.Patch | Should -Be 0
					$json.Revision | Should -Be 0
					$json.Height | Should -Be 2
					$json.HeightPadded | Should -Be '0002'
					$json.Sha | Should -Be $expectedSha
					$json.BranchName | Should -Be 'master'
					$json.Formats.Semver1 | Should -Be '1.0.0-alpha1-0002'
					$json.Formats.Semver2 | Should -Be '1.0.0-alpha1.2'
				}
			}
		}
	}

	Context 'Branch Overrides' {
		Context 'Override Matches' {

			BeforeAll {
				$dir = New-Item "${TestDrive}\$(Get-Random)" -ItemType Directory
				Push-Location $dir
				git init
				git config user.email "SimpleVersion@Kieranties.com"
				git config user.name "Simple Version"

				Copy-Item $PSScriptRoot\assets\.simpleversion.json -Destination $pwd
				git add *
				git commit -m 'Initial commit'

				$sha = git rev-parse HEAD
				$expectedSha = "c$($sha.Substring(0, 7))"
			}

			AfterAll {
				Pop-Location
				Remove-Item $dir -Recurse -Force
			}

			It 'Returns override label and meta if provided' {

				git checkout -b test/feature
				$result = Invoke
				Validate $result -AsSuccess {
					$json.BranchName | Should -Be 'test/feature'
					$json.Formats.Semver1 | Should -Be '0.1.0-testfeature-0001'
					$json.Formats.Semver2 | Should -Be '0.1.0-testfeature.1+internal'
				}
			}

			It 'Returns override label only if provided' {

				git checkout -b test/hotfix
				$result = Invoke
				Validate $result -AsSuccess {
					$json.BranchName | Should -Be 'test/hotfix'
					$json.Formats.Semver1 | Should -Be "0.1.0-$expectedSha-0001"
					$json.Formats.Semver2 | Should -Be "0.1.0-$expectedSha.1"
				}
			}

			It 'Returns override label only if provided' {

				git checkout -b test/release
				$result = Invoke
				Validate $result -AsSuccess {
					$json.BranchName | Should -Be 'test/release'
					$json.Formats.Semver1 | Should -Be "0.1.0-alpha1-0001"
					$json.Formats.Semver2 | Should -Be "0.1.0-alpha1.1+1.$expectedSha"
				}
			}
		}
	}
}