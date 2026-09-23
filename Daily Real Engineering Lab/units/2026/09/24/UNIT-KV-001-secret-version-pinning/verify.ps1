$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/SecretRotationLab.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw "Verification failed. The learner-editable starter does not satisfy the rotation contract." }
Write-Host "PASS: learner-editable starter satisfies the rotation contract."