$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet run --project "$root/starter/Lab.csproj" -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
