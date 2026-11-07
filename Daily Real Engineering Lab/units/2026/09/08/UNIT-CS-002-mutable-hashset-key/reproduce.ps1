$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project
Write-Host $output

if ($output -notmatch "ContainsBeforeSecondAdd=False") { throw "Expected ContainsBeforeSecondAdd=False." }
if ($output -notmatch "SecondAddReturned=True") { throw "Expected SecondAddReturned=True." }
if ($output -notmatch "FinalCount=2") { throw "Expected FinalCount=2." }

Write-Host "REPRODUCED: logical duplicate was accepted by the set."
