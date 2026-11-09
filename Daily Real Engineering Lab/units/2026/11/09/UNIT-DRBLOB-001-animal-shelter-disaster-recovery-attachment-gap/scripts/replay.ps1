param([switch]$Verify)
$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
$fixture = Get-Content (Join-Path $unit 'evidence/recovery-fixture.json') -Raw | ConvertFrom-Json
$decision = Get-Content (Join-Path $unit 'workspace/decision.json') -Raw | ConvertFrom-Json
$dbCutoff = [datetimeoffset]::Parse($fixture.databaseReplicaCutoff)
$blobCutoff = [datetimeoffset]::Parse($fixture.blobReplicaCutoff)
$results = @()
foreach ($case in $fixture.cases) {
  $inDb = [datetimeoffset]::Parse($case.committedAt) -le $dbCutoff
  $blobAvailable = $null -ne $case.blobReplicatedAt -and [datetimeoffset]::Parse($case.blobReplicatedAt) -le $blobCutoff
  $ready = $inDb -and ((-not $decision.enforceAttachmentReadiness) -or $blobAvailable)
  $quarantined = $inDb -and (-not $blobAvailable) -and [bool]$decision.quarantineMissing
  $results += [pscustomobject]@{ Case=$case.id; InDatabase=$inDb; BlobAvailable=$blobAvailable; Visible=$ready; Quarantined=$quarantined }
}
$results | Format-Table | Out-String | Write-Output
if (-not $Verify) { return }
$errors = @()
if ($decision.strategy -notin @('cross-store-watermark','reconcile-and-quarantine')) { $errors += 'Choose a supported cross-store recovery strategy.' }
if (-not $decision.enforceAttachmentReadiness) { $errors += 'Readiness must check mandatory attachment availability.' }
if (-not $decision.quarantineMissing) { $errors += 'Missing mandatory attachments must be accounted for in quarantine.' }
if (-not $decision.requireCrossStoreRecoveryWatermark) { $errors += 'Declare a cross-store recovery watermark.' }
if (@($decision.drillCases | Sort-Object -Unique).Count -ne @($fixture.cases).Count) { $errors += 'Drill must cover every fixture case.' }
foreach ($item in $results) {
  if ($item.Visible -and -not $item.BlobAvailable) { $errors += "Unsafe visible case: $($item.Case)" }
  if ($item.InDatabase -and -not $item.BlobAvailable -and -not $item.Quarantined) { $errors += "Missing quarantine: $($item.Case)" }
  if ($item.InDatabase -and $item.BlobAvailable -and -not $item.Visible) { $errors += "Unnecessarily hidden healthy case: $($item.Case)" }
}
if ($errors.Count -gt 0) { $errors | ForEach-Object { Write-Error $_ }; exit 1 }
Write-Output 'PASS: cross-store recovery contract satisfied.'
