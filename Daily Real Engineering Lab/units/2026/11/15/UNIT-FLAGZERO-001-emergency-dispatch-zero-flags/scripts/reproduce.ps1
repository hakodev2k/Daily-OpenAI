$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$output = & dotnet run --project (Join-Path $root 'starter/DispatchLab.csproj') 2>&1 | Out-String
Write-Output $output
if($output -match 'MISMATCHES=1' -and $output -match 'Unassigned: ACTUAL=True EXPECTED=False'){exit 0}
Write-Error 'Starter symptom not reproduced'; exit 1
