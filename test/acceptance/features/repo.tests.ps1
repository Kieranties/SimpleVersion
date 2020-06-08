BeforeAll {
    . ([IO.Path]::Combine($PSScriptRoot, '..', 'utils.ps1'))
}

Describe "Repository" {
    Context "Not Found" {
        It 'Writes error if repository cannot be found at current path or parent' {
            try {
                $path = Get-Item '~' | Select-Object -ExpandProperty FullName
                Push-Location $path
                Invoke-SimpleVersion | Should -BeError "[Error] Could not find git repository at '${path}' or any parent directory."
            } finally {
                Pop-Location
            }
        }
        It 'Writes error if repository cannot be found at specified path' {
            $path = Get-Item '~' | Select-Object -ExpandProperty FullName
            Invoke-Simpleversion $path | Should -BeError -Message "[Error] Could not find git repository at '${path}' or any parent directory."
        }
    }
}
