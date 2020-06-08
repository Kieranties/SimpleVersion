#Requires -Module @{ ModuleName='Pester'; ModuleVersion='5.0' }

function Invoke-SimpleVersion {
    param(
        [Parameter(ValueFromRemainingArguments)]
        [string[]]$Parameters
    )

    $ErrorActionPreference = 'Continue'
    $errFile = "TestDrive:\$(Get-Random)"

    $result = @{
        Success = $false
        Output = $null
        Error = $null
        ExitCode = -1
    }

    try {
        $output = simpleversion $Parameters 2> $errFile
        if($output) {
            $result.Output = ConvertFrom-Json $output
        }
        $result.Success = $true
    } finally {
        $result.Error = Get-Content $errFile
        $result.Success = -not $result.Error
        $result.ExitCode = $LASTEXITCODE
    }

    $result
}

function BeError{
    param(
        $ActualValue,
        [string]$Message = $null,
        [int]$ExitCode = 1,
        [switch]$Negate
    )

    $negatedStub = $negate ? 'not ' : ''
    $assertMessages = @()
    if($ActualValue.ExitCode -ne $ExitCode) {
        $assertMessages += "Result exit code '$($ActualValue.ExitCode)' should ${negatedStub}match '${ExitCode}'"
    }
    if($Message -and $ActualValue.ErrorMessage -ne $ErrorMessage) {
        $assertMessages += "Result error message '$($ActualValue.ErrorMessage)' should ${negatedStub}match '${Message}'"
    }


    return [PSObject]@{
        Succeeded      = ($assertMessages.Count -eq 0)
        FailureMessage = ($assertMessages -join [Environment]::NewLine)
    }
}

Add-ShouldOperator -Name BeError -Test $function:BeError