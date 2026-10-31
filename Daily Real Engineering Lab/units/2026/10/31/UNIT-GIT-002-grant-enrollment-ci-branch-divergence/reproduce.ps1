$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'setup.ps1')
$scriptPath = Join-Path $PSScriptRoot 'starter/change-detection.ps1'
$repoA = Join-Path $PSScriptRoot 'workspace/fixtures/case-a'
$repoC = Join-Path $PSScriptRoot 'workspace/fixtures/case-c'
$actualA = @(& $scriptPath -RepoPath $repoA -BaseRef main -HeadRef feature)
$actualC = @(& $scriptPath -RepoPath $repoC -BaseRef main -HeadRef feature)
if ('src/Enrollment/EnrollmentService.cs' -notin $actualA) {
    throw 'Fixture case-a không phát hiện thay đổi của feature; kiểm tra setup.'
}
if ('src/Shared/Clock.cs' -notin $actualA) {
    throw 'Không reproduce được file chỉ thuộc base trong case-a. Có thể starter đã sửa.'
}
if ('src/Core/Formatter.cs' -notin $actualC) {
    throw 'Không reproduce được case-c: feature không đổi nhưng script vẫn báo file.'
}
Write-Host 'REPRODUCED: CI trả thêm file không thuộc feature (case-a và case-c).'
