$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RoleClaimLab.csproj'
$output = dotnet run --project $project
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -notmatch 'Authenticated=True') { throw 'Principal không còn authenticated.' }
if ($text -notmatch 'AdminClaimCount=1') { throw 'Admin claim bị mất hoặc thay đổi ngoài yêu cầu.' }
if ($text -notmatch 'IsInRoleAdmin=True') { throw 'Role authorization vẫn chưa nhận diện Admin.' }
Write-Host 'VERIFIED: dữ liệu claim được giữ nguyên và role check đã đúng.'
