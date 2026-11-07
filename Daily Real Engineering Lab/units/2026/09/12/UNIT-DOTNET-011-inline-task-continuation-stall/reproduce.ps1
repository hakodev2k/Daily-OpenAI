$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/InlineContinuationLab.csproj'
$out = Join-Path $env:TEMP 'unit-dotnet-011-reproduce.out.txt'
$err = Join-Path $env:TEMP 'unit-dotnet-011-reproduce.err.txt'
Remove-Item $out,$err -ErrorAction SilentlyContinue

dotnet restore $project | Out-Null
dotnet build $project --no-restore | Out-Null

$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--no-build','--no-restore') -PassThru -NoNewWindow -RedirectStandardOutput $out -RedirectStandardError $err
$exited = $process.WaitForExit(3000)

if (-not $exited) {
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    $text = (Get-Content $out -Raw -ErrorAction SilentlyContinue)
    Write-Host $text
    if ($text -match 'dispatcher: completing signal' -and $text -match 'observer: waiting for gate' -and $text -notmatch '^completed$') {
        Write-Host 'PASS: reproduced stalled completion path.'
        exit 0
    }
    Write-Error 'Process timed out, but expected evidence was not observed.'
    exit 1
}

$text = (Get-Content $out -Raw -ErrorAction SilentlyContinue)
Write-Host $text
Write-Error 'Starter exited instead of reproducing the intended stall. Reset starter code and retry.'
exit 1
