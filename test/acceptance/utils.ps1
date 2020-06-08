#Requires -Module @{ ModuleName='Pester'; ModuleVersion='5.0' }

function New-GitRepository {

    $path = Join-Path $TestDrive (Get-Random)
    New-Item $path -ItemType Directory > $null
    git init $path
    @{
        Path = $path
    }
}

function Invoke-SimpleVersion {
    param(
        [Parameter(ValueFromRemainingArguments)]
        # [AllowEmptyCollection()]
        # [AllowEmptyString()]
        # [AllowNull()]
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
            $result.Output = $output | ConvertFrom-Json
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
        [string]$Message,
        [int]$ExitCode = 1,
        [switch]$Negate = $false
    )

    $negatedStub = $negate ? ' not' : ''
    $Message = "[Error] $Message"
    $assertMessages = @()

    $exitCodeMatch = $ActualValue.ExitCode -ne $ExitCode
    $mesageMatch = $ActualValue.Error -ne $Message
    $emptyOutput = [string]::IsNullOrEmpty($ActualValue.Output)

    if($exitCodeMatch -ne $Negate) {
        $assertMessages += "Exit code '$($ActualValue.ExitCode)' should${negatedStub} match '${ExitCode}'"
    }

    if($mesageMatch -ne $Negate) {
        $assertMessages += "Error message '$($ActualValue.Error)' should${negatedStub} match '${Message}'"
    }

    if($emptyOutput -eq $Negate) {
        $assertMessages += "Output should${negatedStub} be empty"
    }


    return [PSObject]@{
        Succeeded      = ($assertMessages.Count -eq 0)
        FailureMessage = ($assertMessages -join [Environment]::NewLine)
    }
}

Add-ShouldOperator -Name BeError -Test $function:BeError