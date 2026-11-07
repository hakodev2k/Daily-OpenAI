$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet run --project "$root/starter/Lab.csproj" -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
