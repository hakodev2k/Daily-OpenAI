$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Kiosk.csproj'
$output = @(dotnet run --project $project)
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to run' }
$output | ForEach-Object { Write-Output $_ }
if (($output -contains 'second=KIOSK-A:OLD:BOOK-105') -and ($output -contains 'registryVersion=2') -and ($output -contains 'refreshObserved=False')) {
  Write-Output 'REPRODUCED: active configuration differs from printed label'
} else { throw 'Expected baseline symptom not reproduced (starter may have been edited)' }

