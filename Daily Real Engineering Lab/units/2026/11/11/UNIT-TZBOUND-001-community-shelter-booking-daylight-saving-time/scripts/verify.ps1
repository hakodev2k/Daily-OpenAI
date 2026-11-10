$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'tests/Contract/Contract.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Contract failed for learner starter.' }
Write-Output 'VERIFIED'
