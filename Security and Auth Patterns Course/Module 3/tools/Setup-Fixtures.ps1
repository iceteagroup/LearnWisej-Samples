<#
.SYNOPSIS
  AccessOps Module 3 - creates the confidential-file fixtures under ONE approved root, with
  explicit NTFS ACLs, so the scoped read can be proven with an ACL instead of a name.

.DESCRIPTION
  Approved root (default: <module>\AccessOpsData\confidential):
    pool-only\DOC-202-vendor-contract-archive.txt   readable by the pool identity only
    labuser-only\DOC-201-salary-bands-2027.txt      readable by the lab user only
    labuser-only\DOC-203-audit-sample.txt           NOT created: fixture F03 (missing approved file)
    tenant-b\DOC-204-legal-hold.txt                 exists; the policy refuses it (tenant B)

  Lab mode (-PoolAccount and -LabUser): the lesson's setup.
    pool-only      -> PoolAccount read, LabUser nothing
    labuser-only   -> LabUser read,     PoolAccount nothing
    Administrators keep Full control on everything for cleanup; inheritance is removed below the root.
    Example: -PoolAccount "IIS APPPOOL\AccessOps Intranet" -LabUser "LAB\dana.ortiz"

  Local mode (default, no second account): on a developer PC the app process runs as YOU, and the
  browser on the same machine signs in as YOU too. Observed on Windows 11 with Kestrel Negotiate:
  the NTLM loopback logon hands the app a token of your own interactive logon session (same SID,
  same INTERACTIVE group; only ImpersonationLevel differs), so no ACL can separate "pool" from
  "lab user" here. Local mode therefore builds the two outcomes the screen must show:
    labuser-only   -> inherits your Full control            => the scoped read is ALLOWED (DOC-201)
    pool-only      -> DENY read-data to your account (SID)  => the scoped read is an ACL DENIAL (DOC-202),
                      for the process token and the impersonated token alike
  The outcome flip between the two folders needs two different accounts: lab mode only.
  Nothing is created or changed outside the approved root; no account is created.

  -Remove resets the ACLs (as owner) and deletes the approved root.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File tools\Setup-Fixtures.ps1
  powershell -ExecutionPolicy Bypass -File tools\Setup-Fixtures.ps1 -Remove
#>
[CmdletBinding()]
param(
    [string] $Root,
    [string] $PoolAccount,
    [string] $LabUser,
    [switch] $Remove
)

$ErrorActionPreference = 'Stop'
if (-not $Root) { $Root = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) '..\AccessOpsData\confidential' }
$Root =[System.IO.Path]::GetFullPath($Root)
$Admins = '*S-1-5-32-544'          # BUILTIN\Administrators
$System = '*S-1-5-18'              # NT AUTHORITY\SYSTEM
$Me = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name

function Invoke-Icacls {
    & icacls.exe @args | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "icacls $($args -join ' ') failed ($LASTEXITCODE)" }
}

# Back to inherited ACLs (as owner), so the fixtures can be rewritten or deleted.
function Reset-Fixtures {
    if (-not (Test-Path -LiteralPath $Root)) { return }
    foreach ($folder in 'pool-only', 'labuser-only', 'tenant-b') {
        $p = Join-Path $Root $folder
        if (Test-Path -LiteralPath $p) { & icacls.exe $p /reset /C | Out-Null }   # the folder first: then it can be listed
    }
    & icacls.exe $Root /reset /T /C | Out-Null
}

if ($Remove) {
    Reset-Fixtures
    if (Test-Path -LiteralPath $Root) { Remove-Item -LiteralPath $Root -Recurse -Force }
    Write-Host "Removed $Root"
    return
}

Reset-Fixtures   # makes the script safe to run again

$lab = $PoolAccount -and $LabUser
if (($PoolAccount -or $LabUser) -and -not $lab) { throw 'Lab mode needs both -PoolAccount and -LabUser.' }

New-Item -ItemType Directory -Force -Path $Root | Out-Null
$pool = Join-Path $Root 'pool-only'
$user = Join-Path $Root 'labuser-only'
$tenantB = Join-Path $Root 'tenant-b'
New-Item -ItemType Directory -Force -Path $pool, $user, $tenantB | Out-Null

# Fixture content (invented). DOC-201 is about 18 KB, like the walkthrough's "Read 18 KB".
$bands = 1..255 | ForEach-Object { 'Band {0:D3} · grade {1} · range {2:N0} - {3:N0} (fictional lab data)' -f $_, (($_ % 9) + 1), (38000 + $_ * 410), (52000 + $_ * 480) }
Set-Content -LiteralPath (Join-Path $user 'DOC-201-salary-bands-2027.txt') -Value (@('Salary bands 2027 - CONFIDENTIAL - AccessOps lab fixture') + $bands) -Encoding utf8
Set-Content -LiteralPath (Join-Path $pool 'DOC-202-vendor-contract-archive.txt') -Value 'Vendor contract archive - AccessOps lab fixture. Readable by the pool identity only.' -Encoding utf8
Set-Content -LiteralPath (Join-Path $tenantB 'DOC-204-legal-hold.txt') -Value 'Tenant B legal hold - AccessOps lab fixture. The policy must refuse it for tenant A.' -Encoding utf8
Remove-Item -LiteralPath (Join-Path $user 'DOC-203-audit-sample.txt') -ErrorAction SilentlyContinue   # F03 stays missing

# The root: no inherited rights from the module folder. Administrators, SYSTEM and the person
# running the setup keep Full control for cleanup.
Invoke-Icacls $Root /inheritance:r /grant:r "${Admins}:(OI)(CI)F" "${System}:(OI)(CI)F" "${Me}:(OI)(CI)F"

if ($lab) {
    Invoke-Icacls $Root /grant "${PoolAccount}:(RX)" "${LabUser}:(RX)"                       # traverse the root only
    Invoke-Icacls $pool /inheritance:r /grant:r "${Admins}:(OI)(CI)F" "${System}:(OI)(CI)F" "${PoolAccount}:(OI)(CI)R"
    Invoke-Icacls $user /inheritance:r /grant:r "${Admins}:(OI)(CI)F" "${System}:(OI)(CI)F" "${LabUser}:(OI)(CI)R"
    Invoke-Icacls $tenantB /grant "${PoolAccount}:(OI)(CI)R" "${LabUser}:(OI)(CI)R"
}
else {
    $mySid = [System.Security.Principal.WindowsIdentity]::GetCurrent().User.Value
    Invoke-Icacls $pool /deny "*${mySid}:(OI)(CI)(RD)"      # your account may not read data here (owner can still reset)
}

Write-Host "Approved root: $Root  (mode: $(if ($lab) { 'lab' } else { 'local, one account' }))"
foreach ($p in $Root, $pool, $user, $tenantB) { & icacls.exe $p }
