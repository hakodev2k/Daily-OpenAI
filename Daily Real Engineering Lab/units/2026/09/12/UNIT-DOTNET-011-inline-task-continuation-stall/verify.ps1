$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/InlineContinuationLab.csproj'
$out = Join-Path $env:TEMP 'unit-dotnet-011-verify.out.txt'
$err = Join-Path $env:TEMP 'unit-dotnet-011-verify.err.txt'
Remove-Item $out,$err -ErrorAction SilentlyContinue

dotnet restore $project | Out-Null
dotnet build $project --no-restore | Out-Null

$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-build','--no-restore') -PassThru -NoNewWindow -RedirectStandardOutput $out -RedirectStandardError $err
$exited = $process.WaitForExit(5000)

if (-not $exited) {
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    Get-Content $out -ErrorAction SilentlyContinue
    Write-Error 'FAIL: learner code still does not complete within the verification window.'
    exit 1
}

$text = (Get-Content $out -Raw -ErrorAction SilentlyContinue)
Write-Host $text

if ($process.ExitCode -ne 0) {
    Get-Content $err -ErrorAction SilentlyContinue
    Write-Error "FAIL: process exited with code $($process.ExitCode)."
    exit 1
}

$required = @(
    'dispatcher: acquired gate',
    'dispatcher: completing signal',
    'dispatcher: signal completed',
    'dispatcher: released gate',
    'observer: received ready',
    'completed'
)

foreach ($marker in $required) {
    if ($text -notmatch [regex]::Escape($marker)) {
        Write-Error "FAIL: expected behavior marker missing: $marker"
        exit 1
    }
}

Write-Host 'PASS: completion path terminates and observer behavior is preserved.'
