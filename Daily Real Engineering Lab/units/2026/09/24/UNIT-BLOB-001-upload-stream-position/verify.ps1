$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/BlobLab.csproj"
$output | Write-Host
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$input = [int](($output | Where-Object { $_ -like 'InputBytes=*' }) -replace 'InputBytes=', '')
$uploaded = [int](($output | Where-Object { $_ -like 'UploadedBytes=*' }) -replace 'UploadedBytes=', '')
$matches = (($output | Where-Object { $_ -like 'PayloadMatches=*' }) -replace 'PayloadMatches=', '')
$checksum = (($output | Where-Object { $_ -like 'Checksum=*' }) -replace 'Checksum=', '')
if ($input -le 0) { throw 'Input must contain data.' }
if ($uploaded -ne $input) { throw "Expected $input uploaded bytes, got $uploaded." }
if ($matches -ne 'True') { throw 'Stored payload does not match the input.' }
if ([string]::IsNullOrWhiteSpace($checksum)) { throw 'Inspection checksum is missing.' }
Write-Host 'PASS: learner-editable starter preserves inspection and uploads the complete payload.'