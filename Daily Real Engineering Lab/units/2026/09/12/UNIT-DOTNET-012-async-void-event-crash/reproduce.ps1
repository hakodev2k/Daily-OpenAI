$ErrorActionPreference='Stop'
$root=Split-Path -Parent $MyInvocation.MyCommand.Path
$out=Join-Path $env:TEMP 'unit-dotnet-012-repro.out'
$err=Join-Path $env:TEMP 'unit-dotnet-012-repro.err'
$p=Start-Process dotnet -ArgumentList @('run','--project',(Join-Path $root 'starter/AsyncEventLab.csproj')) -Wait -PassThru -RedirectStandardOutput $out -RedirectStandardError $err
$text=((Get-Content $out -Raw -ErrorAction SilentlyContinue)+(Get-Content $err -Raw -ErrorAction SilentlyContinue))
if($p.ExitCode -eq 0){throw 'Expected starter process to terminate with non-zero exit code.'}
if($text -notmatch 'Publish completed'){throw 'Expected publisher to report completion before subscriber failure.'}
if($text -notmatch 'notification provider unavailable'){throw 'Expected unhandled subscriber exception evidence.'}
Write-Host 'PASS: reproduced async subscriber failure escaping the publisher error boundary.'
