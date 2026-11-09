$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'replay.ps1')
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Output 'Quan sát các hồ sơ Visible=True và BlobAvailable=False. Đây là expected starter symptom, không phải verify PASS.'
