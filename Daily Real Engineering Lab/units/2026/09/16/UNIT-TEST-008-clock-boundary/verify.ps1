$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/ClockBoundaryLab.csproj"
$text = $output -join "`n"
if ($text -notmatch 'Before boundary: True') { throw 'Verification failed: one second before expiry must be active.' }
if ($text -notmatch 'At boundary: False') { throw 'Verification failed: at expiry boundary must be inactive.' }
Write-Host 'Verification passed: learner-editable starter obeys the controlled time boundary.'