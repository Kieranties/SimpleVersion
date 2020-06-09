BeforeAll {
    . ([IO.Path]::Combine($PSScriptRoot, '..', 'utils.ps1'))
}

Describe "Configuration" {
    Context "When configuration errors" {

        It "Writes error when it does not parse" {
            $repo = New-Repository {
                Set-Configuration -Value "Invalid json"
                git add -A
                git commit -m 'Initial config'
            }

            Invoke-SimpleVersion $repo | Should -BeError "Could not read '.simpleversion.json', has it been committed?"
        }

        It "Writes error when version is invalid" -TestCases @(
            @{ Version = '1'}, @{ Version = '100.0.0.0.0' }, @{ Version = 'wat' }
        ) {
            $repo = New-Repository {
                Set-Configuration -Object @{ Version = $Version }
                git add -A
                git commit -m 'Initial config'
            }

            Invoke-SimpleVersion $repo | Should -BeError "Version '${Version}' is not in a valid format."
        }
    }

    Context "When minimal configuration" {
        It 'Returns success for major.minor version' {
            $repo = New-Repository {
                Set-Configuration -Object @{ Version = '1.0' }
                git add -A
                git commit -m 'Initial config'
            }
            Invoke-SimpleVersion $repo | Should -Not -BeError
        }
    }
}