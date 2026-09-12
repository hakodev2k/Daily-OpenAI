$repo = Join-Path $PSScriptRoot 'workspace/repo'
if (Test-Path $repo) {
    Remove-Item $repo -Recurse -Force
}
Write-Host 'Workspace reset.'
