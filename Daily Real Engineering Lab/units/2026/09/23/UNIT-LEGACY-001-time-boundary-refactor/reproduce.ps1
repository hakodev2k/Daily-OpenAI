$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$old = $ErrorActionPreference
$ErrorActionPreference = "Continue"
dotnet run --project "$root/starter/TimeBoundaryLab.csproj"
$code = $LASTEXITCODE
$ErrorActionPreference = $old
if ($code -eq 1) { Write-Host "REPRODUCED: recorded incident cannot be replayed at its original instant."; exit 0 }
if ($code -eq 0) { throw "Starter symptom was not observed. Reset starter code and retry." }
throw "Starter failed unexpectedly with exit code $code."