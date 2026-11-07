$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
  dotnet run --project "./starter/Utf8StreamLab.csproj" -- --show
  $result = $LASTEXITCODE
} finally { Pop-Location }
exit $result
