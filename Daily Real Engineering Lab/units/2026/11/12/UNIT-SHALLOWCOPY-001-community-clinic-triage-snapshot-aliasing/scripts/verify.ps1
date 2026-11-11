$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$project=Join-Path $root 'starter/Triage.csproj'
dotnet build $project -nologo
if($LASTEXITCODE -ne 0){throw 'Build failed'}
$output=@(dotnet run --project $project --no-build)
if($LASTEXITCODE -ne 0){throw 'Execution failed'}
$expected=@('before=urgent,routine','after=urgent,routine','live=routine,routine')
foreach($line in $expected){if(-not ($output -ccontains $line)){throw "Expected '$line'; actual: $($output -join '; ')" }}
Write-Host 'PASS: saved snapshot stable, live board remains editable'
