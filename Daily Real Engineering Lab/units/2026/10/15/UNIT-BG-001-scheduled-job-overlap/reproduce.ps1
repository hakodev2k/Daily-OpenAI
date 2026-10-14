$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/baseline/TimerOverlapLab.csproj" -- reproduce
if ($LASTEXITCODE -ne 0) { throw "Baseline failed to reproduce the overlapping batches." }
Write-Host "REPRODUCE=PASS"
