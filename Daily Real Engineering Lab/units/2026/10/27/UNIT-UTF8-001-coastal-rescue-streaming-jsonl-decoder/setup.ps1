$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
  dotnet --version
  if ($LASTEXITCODE -ne 0) { throw 'Requires .NET 10 SDK' }
  foreach ($p in @('baseline','starter','solution')) {
    dotnet restore "./$p/Utf8StreamLab.csproj"
    if ($LASTEXITCODE -ne 0) { throw "Restore failed: $p" }
  }
  Write-Host 'Ready: no cloud or paid dependency.'
} finally { Pop-Location }
