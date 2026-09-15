$ErrorActionPreference='Stop'
$out = dotnet run --project "$PSScriptRoot/starter/CdnVariantLab.csproj"
$out | ForEach-Object { Write-Host $_ }
$bad = @($out | Where-Object { $_ -match 'match=False' }).Count
if ($bad -lt 2) { throw "Expected at least two wrong-variant observations; got $bad." }
Write-Host "Reproduced: edge cache can serve a representation that does not match the request locale."
