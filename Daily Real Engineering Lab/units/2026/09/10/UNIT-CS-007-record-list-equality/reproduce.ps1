$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Lab.csproj"
$output | Write-Output
if ($output -notcontains 'HASHSET_COUNT=2') { throw 'Không reproduce được symptom mong đợi.' }
Write-Output 'REPRODUCE=PASS'
