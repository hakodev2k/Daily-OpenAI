$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
  dotnet run --project "./baseline/Utf8StreamLab.csproj" -- --reproduce
  $result = $LASTEXITCODE
} finally { Pop-Location }
exit $result
