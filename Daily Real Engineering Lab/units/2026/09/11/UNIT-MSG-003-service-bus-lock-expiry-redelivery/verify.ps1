$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Lab.csproj" 2>&1 | Out-String
$output | Write-Host

$deliveries = [int]([regex]::Match($output, 'DELIVERIES=(\d+)').Groups[1].Value)
$maxConcurrent = [int]([regex]::Match($output, 'MAX_CONCURRENT_FOR_MESSAGE=(\d+)').Groups[1].Value)
$sideEffects = [int]([regex]::Match($output, 'SIDE_EFFECTS=(\d+)').Groups[1].Value)
$completions = [int]([regex]::Match($output, 'COMPLETIONS=(\d+)').Groups[1].Value)

if ($deliveries -ne 1) { throw "Expected exactly one delivery, got $deliveries." }
if ($maxConcurrent -ne 1) { throw "Expected max concurrent handler count of 1, got $maxConcurrent." }
if ($sideEffects -ne 1) { throw "Expected exactly one side effect, got $sideEffects." }
if ($completions -ne 1) { throw "Expected exactly one accepted completion, got $completions." }

Write-Host "VERIFIED: long-running processing retains single ownership without duplicate side effects."
