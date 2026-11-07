$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/BlobLab.csproj"
$output | Write-Host
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$uploaded = [int](($output | Where-Object { $_ -like 'UploadedBytes=*' }) -replace 'UploadedBytes=', '')
$matches = (($output | Where-Object { $_ -like 'PayloadMatches=*' }) -replace 'PayloadMatches=', '')
if ($uploaded -ne 0 -or $matches -ne 'False') { throw 'Starter no longer reproduces the intended symptom.' }
Write-Host 'Reproduced: upload reports success but stored payload is empty.'