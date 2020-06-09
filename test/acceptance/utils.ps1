#Requires -Module @{ ModuleName='Pester'; ModuleVersion='5.0' }

function Use-Path {
    param(
        [Parameter(Mandatory)]
        [string]$Path,
        [scriptblock]$Action
    )

    try {
        Push-Location $Path
        $Action.Invoke()
    } finally {
        Pop-Location
    }
}

function Get-Sha {
    param(
        [Parameter(Mandatory)]
        [string]$Path,
        [switch]$Short
    )

    $result = Use-Path $repo { git rev-parse HEAD }
    if($Short) {
        $result = $result.Substring(0, 7)
    }

    $result
}

function Set-Configuration {
    param(
        [Parameter(Mandatory, ParameterSetName = 'default')]
        [AllowNull()]
        [string]$Value,
        [Parameter(Mandatory, ParameterSetName = 'object')]
        [PSObject]$Object
    )

    $fullPath = Join-Path $pwd '.simpleversion.json'
    if('object' -eq $PSCmdlet.ParameterSetName) {
        $Value = ConvertTo-Json $Object -Depth 100
    }
    Set-Content -Path $fullPath -Value $Value
}

function Get-Configuration {

    $fullPath = Join-Path $pwd '.simpleversion.json'
    Get-Content $fullPath -Raw | ConvertFrom-Json
}

function New-Repository {
    param(
        [scriptblock]$Actions = $null
    )

    $path = Join-Path $TestDrive (Get-Random)
    New-Item $path -ItemType Directory > $null
    # TODO: git logs
    git init $path > $null

    if($Actions) {
        Use-Path $path $Actions > $null
    }
    return $path
}

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

    $succeeded = $assertMessages.Count -eq 0
    if(-not $succeeded -and -not $PSBoundParameters['Message']) {
        $assertMessages += $ActualValue.Error
    }

    return [PSObject]@{
        Succeeded      = $succeeded
        FailureMessage = ($assertMessages -join [Environment]::NewLine)
    }
}

Add-ShouldOperator -Name BeError -Test $function:BeError