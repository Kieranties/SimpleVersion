
function Invoke{
    $result = SimpleVersion $Args *>&1
    return [PSCustomObject]@{
        Output = $result
        ExitCode = $LASTEXITCODE
    }
}

Describe 'SimpleVersion.Command'{
    It 'throws error if repository cannot be found'{

        $result = Invoke

        $result.Output | Should Be "[Error] No commits found for '.simpleversion.json'"
        $result.ExitCode | Should Be -1
    }
}