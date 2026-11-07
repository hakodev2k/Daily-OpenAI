$ErrorActionPreference = 'Stop'
$runtime = Join-Path $PSScriptRoot '.lab-runtime'
$seed = Join-Path $runtime 'seed'
$origin = Join-Path $runtime 'origin.git'
$shallow = Join-Path $runtime 'shallow'

function Invoke-LabGit {
    param([string[]] $GitArgs)
    & git @GitArgs | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Git command failed (exit $LASTEXITCODE): git $($GitArgs -join ' ')" }
}

& git --version | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Git CLI not available.' }
if (Test-Path $runtime) { Remove-Item -LiteralPath $runtime -Recurse -Force }
New-Item -ItemType Directory -Path $seed -Force | Out-Null
Invoke-LabGit -GitArgs @('init', $seed)
Invoke-LabGit -GitArgs @('-C', $seed, 'config', 'user.name', 'Engineering Lab')
Invoke-LabGit -GitArgs @('-C', $seed, 'config', 'user.email', 'lab@example.invalid')
New-Item -ItemType Directory -Path (Join-Path $seed 'src/PermitEngine') -Force | Out-Null
Set-Content -Path (Join-Path $seed 'README.md') -Value '# Permit Portal' -Encoding UTF8
Set-Content -Path (Join-Path $seed 'src/PermitEngine/Rules.cs') -Value 'public static class Rules { public const int MaxFloors = 4; }' -Encoding UTF8
Invoke-LabGit -GitArgs @('-C', $seed, 'add', '.')
Invoke-LabGit -GitArgs @('-C', $seed, 'commit', '-m', 'Base permit policy')
Invoke-LabGit -GitArgs @('-C', $seed, 'branch', '-M', 'main')
$baseSha = (& git -C $seed rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot resolve base commit.' }
Invoke-LabGit -GitArgs @('-C', $seed, 'checkout', '-b', 'feature/permit-rule')
Set-Content -Path (Join-Path $seed 'src/PermitEngine/Rules.cs') -Value 'public static class Rules { public const int MaxFloors = 6; }' -Encoding UTF8
New-Item -ItemType Directory -Path (Join-Path $seed 'docs') -Force | Out-Null
Set-Content -Path (Join-Path $seed 'docs/permit-rules.md') -Value '# Permit rule change' -Encoding UTF8
Invoke-LabGit -GitArgs @('-C', $seed, 'add', '.')
Invoke-LabGit -GitArgs @('-C', $seed, 'commit', '-m', 'Update permit rules')
Invoke-LabGit -GitArgs @('init', '--bare', $origin)
Invoke-LabGit -GitArgs @('-C', $seed, 'remote', 'add', 'origin', $origin)
Invoke-LabGit -GitArgs @('-C', $seed, 'push', 'origin', 'main')
Invoke-LabGit -GitArgs @('-C', $seed, 'push', 'origin', 'feature/permit-rule')
$uri = ([System.Uri]::new([System.IO.Path]::GetFullPath($origin))).AbsoluteUri
Invoke-LabGit -GitArgs @('clone', '--quiet', '--depth', '1', '--branch', 'feature/permit-rule', $uri, $shallow)
Set-Content -Path (Join-Path $runtime 'base.sha') -Value $baseSha -Encoding ASCII
$isShallow = (& git -C $shallow rev-parse --is-shallow-repository).Trim()
if ($LASTEXITCODE -ne 0 -or $isShallow -ne 'true') { throw 'Fixture is not a shallow checkout.' }
Write-Output "Fixture ready: shallow checkout, base SHA $baseSha"
