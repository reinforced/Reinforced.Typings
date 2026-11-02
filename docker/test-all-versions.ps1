<#
.SYNOPSIS
    Test builds across multiple .NET SDK versions using Docker containers.

.DESCRIPTION
    This script builds and tests Reinforced.Typings across multiple .NET versions
    using Docker containers, including Mono for .NET Framework 4.8.1 support.
    
    Supported versions:
    - net481: .NET Framework 4.8.1 via Mono
    - net5.0: EOS (legacy support)
    - net6.0: LTS until November 2026
    - net7.0: EOS (legacy support)
    - net8.0: LTS until November 2026  
    - net9.0: STS until May 2026
    - net10.0: LTS until 2028

.PARAMETER Parallel
    Run tests in parallel for faster execution.

.PARAMETER SkipMono
    Skip the Mono/.NET Framework 4.8.1 test.

.PARAMETER Versions
    Specific versions to test (e.g., "8.0", "9.0"). Default: all supported versions.

.EXAMPLE
    ./test-all-versions.ps1
    
.EXAMPLE
    ./test-all-versions.ps1 -Parallel
    
.EXAMPLE
    ./test-all-versions.ps1 -Versions "8.0","9.0" -SkipMono
#>

[CmdletBinding()]
param(
    [switch]$Parallel,
    [switch]$SkipMono,
    [string[]]$Versions
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$TempDir = [System.IO.Path]::GetTempPath()

# .NET SDK versions to test
$AllVersions = [ordered]@{
    "5.0"  = "net5.0"   # EOS Nov 2022 (legacy support)
    "6.0"  = "net6.0"   # LTS until Nov 2026
    "7.0"  = "net7.0"   # EOS May 2024 (legacy support)
    "8.0"  = "net8.0"   # LTS until Nov 2026
    "9.0"  = "net9.0"   # STS until May 2026
    "10.0" = "net10.0"  # LTS until 2028
}

# Filter versions if specified
if ($Versions) {
    $FilteredVersions = [ordered]@{}
    foreach ($v in $Versions) {
        if ($AllVersions.Contains($v)) {
            $FilteredVersions[$v] = $AllVersions[$v]
        } else {
            Write-Host "[WARNING] " -ForegroundColor Yellow -NoNewline
            Write-Host "Unknown or unsupported version: $v (supported: $($AllVersions.Keys -join ', '))"
        }
    }
    $TestVersions = $FilteredVersions
} else {
    $TestVersions = $AllVersions
}

# Results tracking
$script:Results = @{
    Passed = [System.Collections.ArrayList]::new()
    Failed = [System.Collections.ArrayList]::new()
}

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "==========================================" -ForegroundColor Cyan
}

function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] " -ForegroundColor Blue -NoNewline
    Write-Host $Message
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] " -ForegroundColor Green -NoNewline
    Write-Host $Message
}

function Write-TestError {
    param([string]$Message)
    Write-Host "[ERROR] " -ForegroundColor Red -NoNewline
    Write-Host $Message
}

function Test-DockerAvailable {
    try {
        $null = docker version 2>&1
        return $LASTEXITCODE -eq 0
    } catch {
        return $false
    }
}

function Test-DotNetVersion {
    param(
        [string]$SdkVersion,
        [string]$TargetFramework
    )
    
    $ContainerName = "rt-test-$($SdkVersion -replace '\.', '-')"
    $LogFile = Join-Path $TempDir "$ContainerName.log"
    $ErrFile = Join-Path $TempDir "$ContainerName.err"
    
    Write-Info "Testing .NET SDK $SdkVersion ($TargetFramework)..."
    
    # Different .NET versions use different base images
    $ImageSuffix = switch ($SdkVersion) {
        "5.0"  { "-buster-slim" }     # Debian 10
        "7.0"  { "-bullseye-slim" }   # Debian 11
        "10.0" { "-noble" }           # Ubuntu 24.04
        default { "-bookworm-slim" }  # Debian 12 (6.0, 8.0, 9.0)
    }
    
    $buildArgs = @(
        "build",
        "--build-arg", "DOTNET_VERSION=$SdkVersion",
        "--build-arg", "TARGET_FRAMEWORK=$TargetFramework",
        "--build-arg", "IMAGE_SUFFIX=$ImageSuffix",
        "-f", (Join-Path $ScriptDir "Dockerfile.test"),
        "-t", $ContainerName,
        $ProjectRoot
    )
    
    $process = Start-Process -FilePath "docker" -ArgumentList $buildArgs -NoNewWindow -Wait -PassThru -RedirectStandardOutput $LogFile -RedirectStandardError $ErrFile
    
    if ($process.ExitCode -eq 0) {
        Write-Success ".NET SDK $SdkVersion build passed"
        return $true
    } else {
        Write-TestError ".NET SDK $SdkVersion build failed"
        Write-Host "  Log: $LogFile" -ForegroundColor Gray
        Write-Host "  Errors: $ErrFile" -ForegroundColor Gray
        return $false
    }
}

function Test-Mono {
    $ContainerName = "rt-test-net481-mono"
    $LogFile = Join-Path $TempDir "$ContainerName.log"
    $ErrFile = Join-Path $TempDir "$ContainerName.err"
    
    Write-Info "Testing .NET Framework 4.8.1 via Mono..."
    
    $buildArgs = @(
        "build",
        "-f", (Join-Path $ScriptDir "Dockerfile.mono"),
        "-t", $ContainerName,
        $ProjectRoot
    )
    
    $process = Start-Process -FilePath "docker" -ArgumentList $buildArgs -NoNewWindow -Wait -PassThru -RedirectStandardOutput $LogFile -RedirectStandardError $ErrFile
    
    if ($process.ExitCode -eq 0) {
        Write-Success ".NET Framework 4.8.1 (Mono) build passed"
        return $true
    } else {
        Write-TestError ".NET Framework 4.8.1 (Mono) build failed"
        Write-Host "  Log: $LogFile" -ForegroundColor Gray
        Write-Host "  Errors: $ErrFile" -ForegroundColor Gray
        return $false
    }
}

function Invoke-ParallelTests {
    param([hashtable]$Versions)
    
    Write-Info "Running tests in parallel..."
    
    $jobs = @()
    
    foreach ($version in $Versions.Keys) {
        $targetFramework = $Versions[$version]
        $jobs += Start-Job -ScriptBlock {
            param($ScriptDir, $ProjectRoot, $SdkVersion, $TargetFramework, $TempDir)
            
            $ContainerName = "rt-test-$($SdkVersion -replace '\.', '-')"
            $LogFile = Join-Path $TempDir "$ContainerName.log"
            $ErrFile = Join-Path $TempDir "$ContainerName.err"
            
            # Different .NET versions use different base images
            $ImageSuffix = switch ($SdkVersion) {
                "5.0"  { "-buster-slim" }     # Debian 10
                "7.0"  { "-bullseye-slim" }   # Debian 11
                "10.0" { "-noble" }           # Ubuntu 24.04
                default { "-bookworm-slim" }  # Debian 12 (6.0, 8.0, 9.0)
            }
            
            $buildArgs = @(
                "build",
                "--build-arg", "DOTNET_VERSION=$SdkVersion",
                "--build-arg", "TARGET_FRAMEWORK=$TargetFramework",
                "--build-arg", "IMAGE_SUFFIX=$ImageSuffix",
                "-f", (Join-Path $ScriptDir "Dockerfile.test"),
                "-t", $ContainerName,
                $ProjectRoot
            )
            
            $process = Start-Process -FilePath "docker" -ArgumentList $buildArgs -NoNewWindow -Wait -PassThru -RedirectStandardOutput $LogFile -RedirectStandardError $ErrFile
            
            return @{
                Version = $SdkVersion
                Success = ($process.ExitCode -eq 0)
                LogFile = $LogFile
                ErrFile = $ErrFile
            }
        } -ArgumentList $ScriptDir, $ProjectRoot, $version, $targetFramework, $TempDir
    }
    
    # Wait for all jobs and collect results
    $jobResults = $jobs | Wait-Job | Receive-Job
    $jobs | Remove-Job
    
    foreach ($result in $jobResults) {
        if ($result.Success) {
            Write-Success ".NET SDK $($result.Version) build passed"
            [void]$script:Results.Passed.Add($result.Version)
        } else {
            Write-TestError ".NET SDK $($result.Version) build failed"
            Write-Host "  Log: $($result.LogFile)" -ForegroundColor Gray
            Write-Host "  Errors: $($result.ErrFile)" -ForegroundColor Gray
            [void]$script:Results.Failed.Add($result.Version)
        }
    }
}

function Invoke-SequentialTests {
    param([hashtable]$Versions)
    
    foreach ($version in $Versions.Keys | Sort-Object { [version]$_ }) {
        $targetFramework = $Versions[$version]
        
        if (Test-DotNetVersion -SdkVersion $version -TargetFramework $targetFramework) {
            [void]$script:Results.Passed.Add($version)
        } else {
            [void]$script:Results.Failed.Add($version)
        }
        Write-Host ""
    }
}

# Main execution
function Main {
    Write-Header "Reinforced.Typings - Container Testing"
    Write-Info "Project root: $ProjectRoot"
    Write-Info "Temp directory: $TempDir"
    Write-Host ""
    
    # Check Docker availability
    if (-not (Test-DockerAvailable)) {
        Write-TestError "Docker is not available. Please ensure Docker is installed and running."
        exit 1
    }
    
    # Test Mono/.NET Framework first (if not skipped)
    if (-not $SkipMono) {
        Write-Header ".NET Framework 4.8.1 (Mono)"
        if (Test-Mono) {
            [void]$script:Results.Passed.Add("net481-mono")
        } else {
            [void]$script:Results.Failed.Add("net481-mono")
        }
        Write-Host ""
    }
    
    # Test modern .NET versions
    if ($TestVersions.Count -gt 0) {
        Write-Header "Modern .NET Versions"
        
        if ($Parallel) {
            Invoke-ParallelTests -Versions $TestVersions
        } else {
            Invoke-SequentialTests -Versions $TestVersions
        }
    }
    
    # Summary
    Write-Host ""
    Write-Header "TEST SUMMARY"
    
    if ($script:Results.Passed.Count -gt 0) {
        Write-Success "Passed: $($script:Results.Passed -join ', ')"
    }
    
    if ($script:Results.Failed.Count -gt 0) {
        Write-TestError "Failed: $($script:Results.Failed -join ', ')"
        exit 1
    }
    
    Write-Success "All tests passed!"
}

Main
