$PesterPreference = [PesterConfiguration]::Default
$PesterPreference.Should.ErrorAction = 'Continue'

BeforeAll {
    . ([IO.Path]::Combine($PSScriptRoot, '..', 'utils.ps1'))
}

Describe "Branch Overrides" {
    BeforeEach {
        $repo = New-Repository {
            Set-Configuration -Object @{
                Version = "0.1.0"
                Label = @("alpha1")
                Branches = @{
                    Release = @(
                        "^refs/heads/master$",
                        "^refs/heads/release/.+$",
                        "^refs/heads/test/.+$"
                    )
                    Overrides = @(
                        @{
                            Match = "test/feature"
                            Label = @("{shortbranchname}")
                            Metadata = @("internal")
                        },
                        @{
                            Match = "test/release"
                            Metadata = @("*", "{shortsha}")
                        },
                        @{
                            Match = "test/hotfix"
                            Label = @("{shortsha}")
                        }
                    )
                }
            }
            git add -A
            git commit -m 'Initial config'
        }

        $expectedSha = "c$(Get-Sha $repo -Short)"
    }

    It 'Returns override label and meta if provided' {
        Use-Path $repo {
            git checkout -b test/feature
        }

        $result = Invoke-SimpleVersion $repo

        $result | Should -Not -BeError
        $result.Output.BranchName | Should -Be 'test/feature'
        $result.Output.Formats.Semver1 | Should -Be '0.1.0-testfeature-0001'
        $result.Output.Formats.Semver2 | Should -Be '0.1.0-testfeature.1+internal'

    }

    It 'Returns override label only if provided' {
        Use-Path $repo {
            git checkout -b test/hotfix
        }

        $result = Invoke-SimpleVersion $repo

        $result | Should -Not -BeError
        $result.Output.BranchName | Should -Be 'test/hotfix'
        $result.Output.Formats.Semver1 | Should -Be "0.1.0-$expectedSha-0001"
        $result.Output.Formats.Semver2 | Should -Be "0.1.0-$expectedSha.1"

    }

    It 'Returns override metadata only if provided' {
        Use-Path $repo {
            git checkout -b test/release
        }

        $result = Invoke-SimpleVersion $repo

        $result | Should -Not -BeError
        $result.Output.BranchName | Should -Be 'test/release'
        $result.Output.Formats.Semver1 | Should -Be "0.1.0-alpha1-0001"
        $result.Output.Formats.Semver2 | Should -Be "0.1.0-alpha1.1+1.$expectedSha"
    }
}