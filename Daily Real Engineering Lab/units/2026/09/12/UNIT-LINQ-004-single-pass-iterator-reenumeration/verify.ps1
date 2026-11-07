$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

function Assert-Case {
    param(
        [string]$InputValues,
        [string]$ExpectedTotal,
        [int]$ExpectedHighRiskCount
    )

    $output = & dotnet run --project $project -- $InputValues 2>&1
    if ($LASTEXITCODE -ne 0) {
        $output | Write-Host
        throw "Application failed for $InputValues"
    }

    $text = $output -join "`n"
    $output | Write-Host

    if (-not $text.Contains("Total=$ExpectedTotal")) {
        throw "Wrong total for $InputValues"
    }

    if (-not $text.Contains("HighRiskCount=$ExpectedHighRiskCount")) {
        throw "Wrong high-risk count for $InputValues"
    }
}

Assert-Case -InputValues '200,1200,1500' -ExpectedTotal '2900' -ExpectedHighRiskCount 2
Assert-Case -InputValues '1000,50,2000,400' -ExpectedTotal '3450' -ExpectedHighRiskCount 2

Write-Host 'Verification passed.'
