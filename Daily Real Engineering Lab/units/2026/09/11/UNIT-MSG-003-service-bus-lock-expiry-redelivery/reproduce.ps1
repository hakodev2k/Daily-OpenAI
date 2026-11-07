$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Lab.csproj" 2>&1 | Out-String
$output | Write-Host

$deliveries = [int]([regex]::Match($output, 'DELIVERIES=(\d+)').Groups[1].Value)
$maxConcurrent = [int]([regex]::Match($output, 'MAX_CONCURRENT_FOR_MESSAGE=(\d+)').Groups[1].Value)
$sideEffects = [int]([regex]::Match($output, 'SIDE_EFFECTS=(\d+)').Groups[1].Value)

if ($deliveries -lt 2 -or $maxConcurrent -lt 2 -or $sideEffects -lt 2) {
    throw "Expected original redelivery symptom was not reproduced."
}

Write-Host "REPRODUCED: same message was processed by overlapping deliveries."
