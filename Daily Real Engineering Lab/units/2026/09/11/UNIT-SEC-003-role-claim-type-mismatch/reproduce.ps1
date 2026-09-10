$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RoleClaimLab.csproj'
$output = dotnet run --project $project
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -notmatch 'Authenticated=True' -or $text -notmatch 'AdminClaimCount=1' -or $text -notmatch 'IsInRoleAdmin=False') {
    throw 'Không reproduce được symptom mong đợi của starter.'
}
Write-Host 'REPRODUCED: principal có Admin claim nhưng role check vẫn False.'
