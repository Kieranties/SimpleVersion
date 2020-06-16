BeforeAll {
    . ([IO.Path]::Combine($PSScriptRoot, '..', 'utils.ps1'))
}

Describe "Single Branch" {
    BeforeEach {
        $repo = New-Repository {
            Set-Configuration -Object @{
                Version = "0.1.0"
                Label = @("alpha1")
                Branches = @{
                    Release = @("^refs/heads/master$")
                }
            }
            git add -A
            git commit -m 'Initial config'
        }
    }

    It 'Returns base values for initial commit' {
        $expectedSha = Get-Sha $repo

        $result = Invoke-SimpleVersion $repo

        $result | Should -Not -BeError
        $result.Output.Version | Should -Be '0.1.0'
        $result.Output.Major | Should -Be 0
        $result.Output.Minor | Should -Be 1
        $result.Output.Patch | Should -Be 0
        $result.Output.Revision | Should -Be 0
        $result.Output.Height | Should -Be 1
        $result.Output.HeightPadded | Should -Be '0001'
        $result.Output.Sha | Should -Be $expectedSha
        $result.Output.BranchName | Should -Be 'master'
        $result.Output.Formats.Semver1 | Should -Be '0.1.0-alpha1-0001'
        $result.Output.Formats.Semver2 | Should -Be '0.1.0-alpha1.1'
    }

    It 'Increments for each commit' {
        Use-Path $repo {
            1..4 | ForEach-Object {
                git commit -m 'empty' --allow-empty
            }
        }
        $expectedSha = Get-Sha $repo

        $result = Invoke-SimpleVersion $repo

        $result | Should -Not -BeError
        $result.Output.Version | Should -Be '0.1.0'
        $result.Output.Major | Should -Be 0
        $result.Output.Minor | Should -Be 1
        $result.Output.Patch | Should -Be 0
        $result.Output.Revision | Should -Be 0
        $result.Output.Height | Should -Be 5
        $result.Output.HeightPadded | Should -Be '0005'
        $result.Output.Sha | Should -Be $expectedSha
        $result.Output.BranchName | Should -Be 'master'
        $result.Output.Formats.Semver1 | Should -Be '0.1.0-alpha1-0005'
        $result.Output.Formats.Semver2 | Should -Be '0.1.0-alpha1.5'
    }

    It 'Resets on version change' {
        Use-Path $repo {
            git commit -m 'empty' --allow-empty
            git commit -m 'empty' --allow-empty
            $config = Get-Configuration
            $config.Version = "1.0.0"
            Set-Configuration -Object $config
            git commit -a -m 'Updated version'
            git commit -m 'empty' --allow-empty
        }

        $expectedSha = Get-Sha $repo

        $result = Invoke-SimpleVersion $repo

        $result | Should -Not -BeError
        $result.Output.Version | Should -Be '1.0.0'
        $result.Output.Major | Should -Be 1
        $result.Output.Minor | Should -Be 0
        $result.Output.Patch | Should -Be 0
        $result.Output.Revision | Should -Be 0
        $result.Output.Height | Should -Be 2
        $result.Output.HeightPadded | Should -Be '0002'
        $result.Output.Sha | Should -Be $expectedSha
        $result.Output.BranchName | Should -Be 'master'
        $result.Output.Formats.Semver1 | Should -Be '1.0.0-alpha1-0002'
        $result.Output.Formats.Semver2 | Should -Be '1.0.0-alpha1.2'
    }
}