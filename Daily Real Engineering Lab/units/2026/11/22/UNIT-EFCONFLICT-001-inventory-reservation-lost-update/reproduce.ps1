$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/SeatLab.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$output
if ($output -notmatch 'accepted=2 remaining=0 version=3') { throw 'Expected starter symptom not reproduced. Reset starter or inspect changes.' }
Write-Host 'REPRODUCED: two accepted for one seat'
