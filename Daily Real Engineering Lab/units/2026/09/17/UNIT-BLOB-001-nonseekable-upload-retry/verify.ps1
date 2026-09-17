$ErrorActionPreference='Stop'
$out = dotnet run --project "$PSScriptRoot/starter/BlobRetryLab.csproj"
$out | Write-Host
if ($out -notmatch 'attempts=2') { throw 'Expected one retry.' }
if ($out -notmatch 'reads=25,25') { throw 'Each upload attempt must receive the complete payload.' }
if ($out -notmatch 'stored-bytes=25') { throw 'Stored payload length is incorrect.' }
if ($out -notmatch 'stored=invoice-pdf-content-12345') { throw 'Stored payload content is incorrect.' }
Write-Host 'PASS: learner starter preserves payload across retry.'