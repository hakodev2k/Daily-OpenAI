$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$output = & dotnet run --project $project -- '200,1200,1500' 2>&1
if ($LASTEXITCODE -ne 0) {
    $output | Write-Host
    exit 1
}
$text = $output -join "`n"
$output | Write-Host
if ($text.Contains('Total=2900') -and $text.Contains('HighRiskCount=0')) {
    Write-Host 'Reproduction confirmed: aggregate total is present but derived high-risk view is empty.'
    exit 0
}
Write-Error 'Expected starter symptom was not reproduced.'
exit 1
