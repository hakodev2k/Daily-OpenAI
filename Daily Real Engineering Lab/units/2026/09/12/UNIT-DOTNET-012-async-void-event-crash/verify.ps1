$ErrorActionPreference='Stop'
$root=Split-Path -Parent $MyInvocation.MyCommand.Path
$out=Join-Path $env:TEMP 'unit-dotnet-012-verify.out'
$err=Join-Path $env:TEMP 'unit-dotnet-012-verify.err'
$p=Start-Process dotnet -ArgumentList @('run','--project',(Join-Path $root 'starter/AsyncEventLab.csproj')) -Wait -PassThru -RedirectStandardOutput $out -RedirectStandardError $err
$text=((Get-Content $out -Raw -ErrorAction SilentlyContinue)+(Get-Content $err -Raw -ErrorAction SilentlyContinue))
if($p.ExitCode -ne 0){throw "Expected fixed starter to exit 0, got $($p.ExitCode)."}
if($text -notmatch 'Publisher caught:'){throw 'Expected publisher error boundary to observe subscriber failure.'}
if($text -notmatch 'Process finished normally'){throw 'Expected normal process completion.'}
if($text -match 'Publish completed'){throw 'Failed publish must not be reported as successful.'}
Write-Host 'PASS: learner starter exposes subscriber completion/failure through an awaitable contract.'
