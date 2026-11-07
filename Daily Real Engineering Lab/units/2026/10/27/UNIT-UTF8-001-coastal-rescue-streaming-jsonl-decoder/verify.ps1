$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
  dotnet run --project "./starter/Utf8StreamLab.csproj" -- --verify
  $result = $LASTEXITCODE
} finally { Pop-Location }
exit $result
