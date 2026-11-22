$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot '../starter/DispatchLab.csproj'
$output=dotnet run --project $project 2>&1 | Out-String
if($LASTEXITCODE -ne 0){throw "Starter failed to run: $output"}
Write-Output $output
if($output -match 'EXPECTED=4 ACTUAL=2' -and $output -match 'accepted=False'){Write-Output 'REPRODUCED: lost requests';exit 0}
throw 'Expected starter symptom not observed; inspect learner modifications.'
