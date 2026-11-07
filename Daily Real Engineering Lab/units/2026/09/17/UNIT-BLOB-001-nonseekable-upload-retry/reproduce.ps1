$ErrorActionPreference='Stop'
$out = dotnet run --project "$PSScriptRoot/starter/BlobRetryLab.csproj"
$out | Write-Host
if ($out -notmatch 'attempts=2' -or $out -notmatch 'reads=25,0' -or $out -notmatch 'stored-bytes=0') { throw 'Starter did not reproduce the intended failure.' }
Write-Host 'Reproduced: retry reports success but stored payload is empty.'