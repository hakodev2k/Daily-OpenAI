$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/AuditLab.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
