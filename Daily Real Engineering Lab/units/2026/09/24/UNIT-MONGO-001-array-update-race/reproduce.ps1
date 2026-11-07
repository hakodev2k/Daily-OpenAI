$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/MongoRaceLab.csproj"
$output | ForEach-Object { Write-Host $_ }
$last = ($output | Select-Object -Last 1)
if ($last -match 'alice' -and $last -match 'bob') { throw 'Expected starter to demonstrate a lost concurrent change, but both members survived.' }
Write-Host 'Reproduction confirmed: successful concurrent operations did not preserve both membership changes.'