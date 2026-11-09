<#
.SYNOPSIS
  AccessOps Module 7 - the release publish step: one Release build for both IIS applications, with
  "secure": true written into the PUBLISHED Default.json (the project's Default.json stays as the
  developer uses it on http://localhost).

.DESCRIPTION
  1. dotnet publish -c Release (pinned packages from AccessOps.csproj: Wisej-4 / Wisej-4-WebAuthn 4.1.7,
     ASP.NET Core 10.0.14). #if DEBUG code - the release review, the software authenticator, the
     demo password hashes and the fault injection - is not in the build.
  2. Default.json in the output gets "secure": true (the Wisej.NET client then only runs over HTTPS).
  3. Checks: no appsettings.Development.json, Default.json says secure, the review type is absent
     from AccessOps.dll, web.config is there (IIS in-process hosting).
  The same folder is deployed to both IIS applications; each sets its route with an environment
  variable in its web.config / app pool:  AccessOps__Route = Intranet  or  AccessOps__Route = Partner.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File tools\Publish-Release.ps1 -Output C:\Temp\accessops-release
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $Output
)
$ErrorActionPreference = 'Stop'
$module = Split-Path -Parent $PSScriptRoot
$project = Join-Path $module 'AccessOps\AccessOps.csproj'

dotnet publish $project -c Release -o $Output --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed ($LASTEXITCODE)" }

$defaultJson = Join-Path $Output 'Default.json'
if (-not (Test-Path $defaultJson)) { throw "Default.json is missing from the publish output" }
$config = Get-Content $defaultJson -Raw | ConvertFrom-Json
$config | Add-Member -NotePropertyName secure -NotePropertyValue $true -Force
$config | ConvertTo-Json -Depth 5 | Set-Content $defaultJson -Encoding UTF8

$checks = [ordered]@{
    'Default.json "secure": true'               = ((Get-Content $defaultJson -Raw | ConvertFrom-Json).secure -eq $true)
    'no appsettings.Development.json'           = -not (Test-Path (Join-Path $Output 'appsettings.Development.json'))
    'web.config present (IIS in-process)'       = (Test-Path (Join-Path $Output 'web.config'))
    'no ReleaseReviewView in AccessOps.dll'     = -not (Select-String -Path (Join-Path $Output 'AccessOps.dll') -Pattern 'ReleaseReviewView' -SimpleMatch -Quiet)
    'no demo password in AccessOps.dll'         = -not (Select-String -Path (Join-Path $Output 'AccessOps.dll') -Pattern 'Reports-demo-31' -SimpleMatch -Quiet -Encoding Unicode)
    'no fault injection in AccessOps.dll'       = -not (Select-String -Path (Join-Path $Output 'AccessOps.dll') -Pattern 'ReplayableHandoff' -SimpleMatch -Quiet -Encoding Unicode)
}
$checks.GetEnumerator() | ForEach-Object { '{0,-42} {1}' -f $_.Key, $(if ($_.Value) { 'OK' } else { 'FAILED' }) }
if ($checks.Values -contains $false) { throw 'A release check failed.' }
"Published to $Output - deploy the same folder to both IIS applications (AccessOps__Route = Intranet | Partner)."
