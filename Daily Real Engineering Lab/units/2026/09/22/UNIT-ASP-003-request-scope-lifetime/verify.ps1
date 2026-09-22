$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- --mode verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: request state is still crossing the intended scope boundary.' }
Write-Host 'Verification passed.'