$ErrorActionPreference = 'Stop'
$fixtureRoot = Join-Path $PSScriptRoot 'workspace/fixtures'
if (Test-Path -LiteralPath $fixtureRoot) {
    Remove-Item -LiteralPath $fixtureRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $fixtureRoot -Force | Out-Null
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw 'Không tìm thấy Git CLI trong PATH.'
}
function Invoke-Git {
    param([string]$Repo, [string[]]$GitArgs)
    & git -C $Repo @GitArgs | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "git $($GitArgs -join ' ') thất bại tại $Repo." }
}
function Write-RepoFile {
    param([string]$Repo, [string]$RelativePath, [string]$Content)
    $path = Join-Path $Repo $RelativePath
    New-Item -ItemType Directory -Path (Split-Path $path -Parent) -Force | Out-Null
    Set-Content -LiteralPath $path -Value $Content -Encoding UTF8
}
function New-Scenario {
    param(
        [string]$Name, [string]$BasePath, [string]$BaseText,
        [string]$FeaturePath, [string]$FeatureText,
        [string]$MainPath, [string]$MainText
    )
    $repo = Join-Path $fixtureRoot $Name
    New-Item -ItemType Directory -Path $repo -Force | Out-Null
    & git init -q -b main $repo | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "git init thất bại: $repo" }
    Invoke-Git $repo @('config','user.name','Engineering Lab')
    Invoke-Git $repo @('config','user.email','lab@example.invalid')
    Write-RepoFile $repo $BasePath $BaseText
    Invoke-Git $repo @('add','-A')
    Invoke-Git $repo @('commit','-q','-m','baseline')
    Invoke-Git $repo @('switch','-q','-c','feature')
    if (-not [string]::IsNullOrWhiteSpace($FeaturePath)) {
        Write-RepoFile $repo $FeaturePath $FeatureText
        Invoke-Git $repo @('add','-A')
        Invoke-Git $repo @('commit','-q','-m','feature-work')
    }
    Invoke-Git $repo @('switch','-q','main')
    Write-RepoFile $repo $MainPath $MainText
    Invoke-Git $repo @('add','-A')
    Invoke-Git $repo @('commit','-q','-m','base-advanced')
}
New-Scenario -Name 'case-a' -BasePath 'src/Shared/Clock.cs' -BaseText 'public class Clock { public int Value => 1; }' -FeaturePath 'src/Enrollment/EnrollmentService.cs' -FeatureText 'public class EnrollmentService { }' -MainPath 'src/Shared/Clock.cs' -MainText 'public class Clock { public int Value => 2; }'
New-Scenario -Name 'case-b' -BasePath 'src/Api/Approval.cs' -BaseText 'public class Approval { public bool Enabled => false; }' -FeaturePath 'src/Api/Approval.cs' -FeatureText 'public class Approval { public bool Enabled => true; }' -MainPath 'src/Features/ReleaseNote.cs' -MainText 'public class ReleaseNote { }'
New-Scenario -Name 'case-c' -BasePath 'src/Core/Formatter.cs' -BaseText 'public class Formatter { public string Name => "v1"; }' -FeaturePath '' -FeatureText '' -MainPath 'src/Core/Formatter.cs' -MainText 'public class Formatter { public string Name => "v2"; }'
Write-Host 'Đã tạo ba Git fixture độc lập tại workspace/fixtures.'
