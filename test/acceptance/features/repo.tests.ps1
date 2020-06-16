BeforeAll {
    . ([IO.Path]::Combine($PSScriptRoot, '..', 'utils.ps1'))
}

Describe "Repository" {

    Context "When invalid path" {
        It 'Writes error for white space path' {
            $path = "`t`t  "
            $expectedError = "'${path}' is not a valid working directory. (Parameter 'workingDirectory')"
            Invoke-Simpleversion $path | Should -BeError $expectedError
        }

        It 'Writes error for non-directory path' {
            $testFile = Join-Path $TestDrive "$(Get-Random).txt"
            New-Item $testFile -ItemType File > $null
            $expectedError = "Could not find directory '${testFile}'"
            Invoke-Simpleversion $testFile | Should -BeError $expectedError
        }

        It 'Writes error for non-existing path' {
            $testPath = Join-Path $TestDrive (Get-Random)
            $expectedError = "Could not find directory '${testPath}'"
            Invoke-Simpleversion $testPath | Should -BeError $expectedError
        }
    }

    Context "When missing" {
        BeforeAll {
            $path = Get-Item '~' | Select-Object -ExpandProperty FullName
            $expectedError = "Could not find git repository at '${path}' or any parent directory."
        }

        It 'Writes error if not found in working directory or parent' {
            try {
                Push-Location $path
                Invoke-SimpleVersion | Should -BeError $expectedError
            } finally {
                Pop-Location
            }
        }

        It 'Writes error if not found at specified path' {
            Invoke-Simpleversion $path | Should -BeError $expectedError
        }
    }

    Context "When exists" {
        Context "With no commits" {
            BeforeEach {
                $repo = New-Repository
                $expectedError = 'Could not find the current branch tip. Unable to identify the current commit.'
            }

            It "Writes error when in the working directory" {
                try {
                    Push-Location $repo
                    Invoke-SimpleVersion | Should -BeError $expectedError
                } finally {
                    Pop-Location
                }
            }

            It "Writes error when in a parent directory" {
                $child = New-Item (Join-Path $repo (Get-Random)) -ItemType Directory
                Invoke-Simpleversion $child.FullName | Should -BeError $expectedError
            }
        }

        Context "With no configuration" {
            BeforeEach {
                $repo = New-Repository {
                    git commit -m 'init' --allow-empty
                }

                $expectedError = "Could not read '.simpleversion.json', has it been committed?"
            }

            It "Writes error when in the working directory" {
                try {
                    Push-Location $repo
                    Invoke-SimpleVersion | Should -BeError $expectedError
                } finally {
                    Pop-Location
                }
            }

            It "Writes error when in a parent directory" {
                $child = New-Item (Join-Path $repo (Get-Random)) -ItemType Directory
                Invoke-Simpleversion $child.FullName | Should -BeError $expectedError
            }
        }
    }
}
