# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

[CmdletBinding()]
param (
    [Parameter(Mandatory = $False)]
    [String]$Build = '0.0.1',

    [Parameter(Mandatory = $False)]
    [String]$Configuration = 'Debug',

    [Parameter(Mandatory = $False)]
    [String]$ApiKey,

    [Parameter(Mandatory = $False)]
    [Switch]$CodeCoverage = $False,

    [Parameter(Mandatory = $False)]
    [String]$ArtifactPath = (Join-Path -Path $PWD -ChildPath out/modules),

    [Parameter(Mandatory = $False)]
    [String]$AssertStyle = 'AzurePipelines',

    [Parameter(Mandatory = $False)]
    [String]$TestGroup = $Null
)

Write-Host -Object "[Pipeline] -- PowerShell: v$($PSVersionTable.PSVersion.ToString())" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- PWD: $PWD" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- ArtifactPath: $ArtifactPath" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- BuildNumber: $($Env:BUILD_BUILDNUMBER)" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- SourceBranch: $($Env:BUILD_SOURCEBRANCH)" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- SourceBranchName: $($Env:BUILD_SOURCEBRANCHNAME)" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- Culture: $((Get-Culture).Name), $((Get-Culture).Parent)" -ForegroundColor Green;

if ($Env:SYSTEM_DEBUG -eq 'true') {
    $VerbosePreference = 'Continue';
}

$forcePublish = $False;
if ($Env:FORCE_PUBLISH -eq 'true') {
    $forcePublish = $True;
}

if ($Env:BUILD_SOURCEBRANCH -like '*/tags/*' -and $Env:BUILD_SOURCEBRANCHNAME -like 'v0.*') {
    $Build = $Env:BUILD_SOURCEBRANCHNAME.Substring(1);
}

$version = $Build;
$versionSuffix = [String]::Empty;

if ($version -like '*-*') {
    [String[]]$versionParts = $version.Split('-', [System.StringSplitOptions]::RemoveEmptyEntries);
    $version = $versionParts[0];

    if ($versionParts.Length -eq 2) {
        $versionSuffix = $versionParts[1];
    }
}

Write-Host -Object "[Pipeline] -- Using version: $version" -ForegroundColor Green;
Write-Host -Object "[Pipeline] -- Using versionSuffix: $versionSuffix" -ForegroundColor Green;

if ($Env:COVERAGE -eq 'true') {
    $CodeCoverage = $True;
}

# Copy the PowerShell modules files to the destination path
function CopyModuleFiles {
    param (
        [Parameter(Mandatory = $True)]
        [String]$Path,

        [Parameter(Mandatory = $True)]
        [String]$DestinationPath
    )

    process {
        $sourcePath = Resolve-Path -Path $Path;

        Get-ChildItem -Path $sourcePath -Recurse -File -Include *.ps1,*.psm1,*.psd1,*.ps1xml,*.yaml | Where-Object -FilterScript {
            ($_.FullName -notmatch '(\.(cs|csproj)|(\\|\/)(obj|bin))')
        } | ForEach-Object -Process {
            $filePath = $_.FullName.Replace($sourcePath, $destinationPath);

            $parentPath = Split-Path -Path $filePath -Parent;

            if (!(Test-Path -Path $parentPath)) {
                $Null = New-Item -Path $parentPath -ItemType Directory -Force;
            }

            Copy-Item -Path $_.FullName -Destination $filePath -Force;
        };
    }
}

task VersionModule ModuleDependencies, {
    $modulePath = Join-Path -Path $ArtifactPath -ChildPath PSRule.Rules.GitHub;
    $manifestPath = Join-Path -Path $modulePath -ChildPath PSRule.Rules.GitHub.psd1;
    Write-Verbose -Message "[VersionModule] -- Checking module path: $modulePath";

    if (![String]::IsNullOrEmpty($Build)) {
        # Update module version
        if (![String]::IsNullOrEmpty($version)) {
            Write-Verbose -Message "[VersionModule] -- Updating module manifest ModuleVersion";
            Update-ModuleManifest -Path $manifestPath -ModuleVersion $version;
        }

        # Update pre-release version
        if (![String]::IsNullOrEmpty($versionSuffix)) {
            Write-Verbose -Message "[VersionModule] -- Updating module manifest Prerelease";
            Update-ModuleManifest -Path $manifestPath -Prerelease $versionSuffix;
        }
    }

    $dependencies = Get-Content -Path $PWD/modules.json -Raw | ConvertFrom-Json;
    $manifest = Test-ModuleManifest -Path $manifestPath;
    $requiredModules = $manifest.RequiredModules | ForEach-Object -Process {
        if ($_.Name -eq 'PSRule' -and $Configuration -eq 'Release') {
            @{ ModuleName = 'PSRule'; ModuleVersion = $dependencies.dependencies.PSRule.version }
        }
        else {
            @{ ModuleName = $_.Name; ModuleVersion = $_.Version }
        }
    };
    Update-ModuleManifest -Path $manifestPath -RequiredModules $requiredModules;
}

# Synopsis: Publish to PowerShell Gallery
task ReleaseModule VersionModule, {
    $modulePath = (Join-Path -Path $ArtifactPath -ChildPath PSRule.Rules.GitHub);
    Write-Verbose -Message "[ReleaseModule] -- Checking module path: $modulePath";

    if (!(Test-Path -Path $modulePath)) {
        Write-Error -Message "[ReleaseModule] -- Module path does not exist";
    }
    elseif (![String]::IsNullOrEmpty($ApiKey)) {
        Publish-Module -Path $modulePath -NuGetApiKey $ApiKey -Force:$forcePublish;
    }
}

# Synopsis: Install NuGet provider
task NuGet {
    if ($Null -eq (Get-PackageProvider -Name NuGet -ErrorAction Ignore)) {
        Install-PackageProvider -Name NuGet -Force -Scope CurrentUser;
    }
}

# Synopsis: Install module dependencies
task ModuleDependencies Dependencies, {
}

task BuildDotNet {
    exec {
        # Build library
        dotnet build src/SDK -c $Configuration -f netstandard2.0 -p:version=$Build
        dotnet publish src/PSRule.Rules.GitHub -c $Configuration -f netstandard2.0 -o $(Join-Path -Path $PWD -ChildPath out/modules/PSRule.Rules.GitHub) -p:version=$Build
    }
}

task TestDotNet {
    dotnet test
}

task CopyModule {
    CopyModuleFiles -Path src/PSRule.Rules.GitHub -DestinationPath out/modules/PSRule.Rules.GitHub;

    # Copy third party notices
    Copy-Item -Path ThirdPartyNotices.txt -Destination out/modules/PSRule.Rules.GitHub;
}

# Synopsis: Build modules only
task BuildModule BuildDotNet, CopyModule

task TestModule ModuleDependencies, {
    # Run Pester tests
    $pesterOptions = @{
        Run = @{
            Path = (Join-Path -Path $PWD -ChildPath tests/PSRule.Rules.GitHub.Tests);
            PassThru = $True;
        };
        TestResult = @{
            Enabled = $True;
            OutputFormat = 'NUnitXml';
            OutputPath = 'reports/pester-unit.xml';
        };
    };

    if ($CodeCoverage) {
        $codeCoverageOptions = @{
            Enabled = $True;
            OutputPath = (Join-Path -Path $PWD -ChildPath 'reports/pester-coverage.xml');
            Path = (Join-Path -Path $PWD -ChildPath 'out/modules/**/*.psm1');
        };

        $pesterOptions.Add('CodeCoverage', $codeCoverageOptions);
    }

    if (!(Test-Path -Path reports)) {
        $Null = New-Item -Path reports -ItemType Directory -Force;
    }

    if ($Null -ne $TestGroup) {
        $pesterOptions.Add('Filter', @{ Tag = $TestGroup });
    }

    # https://pester.dev/docs/commands/New-PesterConfiguration
    $pesterConfiguration = New-PesterConfiguration -Hashtable $pesterOptions;

    $results = Invoke-Pester -Configuration $pesterConfiguration;

    # Throw an error if pester tests failed
    if ($Null -eq $results) {
        throw 'Failed to get Pester test results.';
    }
    elseif ($results.FailedCount -gt 0) {
        throw "$($results.FailedCount) tests failed.";
    }
}

task IntegrationTest ModuleDependencies, {
    # Run Pester tests
    $pesterOptions = @{
        Run = @{
            Path = (Join-Path -Path $PWD -ChildPath tests/Integration);
            PassThru = $True;
        };
        TestResult = @{
            Enabled = $True;
            OutputFormat = 'NUnitXml';
            OutputPath = 'reports/pester-unit.xml';
        };
    };

    if ($CodeCoverage) {
        $codeCoverageOptions = @{
            Enabled = $True;
            OutputPath = (Join-Path -Path $PWD -ChildPath 'reports/pester-coverage.xml');
            Path = (Join-Path -Path $PWD -ChildPath 'out/modules/**/*.psm1');
        };

        $pesterOptions.Add('CodeCoverage', $codeCoverageOptions);
    }

    if (!(Test-Path -Path reports)) {
        $Null = New-Item -Path reports -ItemType Directory -Force;
    }

    if ($Null -ne $TestGroup) {
        $pesterOptions.Add('Filter', @{ Tag = $TestGroup });
    }

    # https://pester.dev/docs/commands/New-PesterConfiguration
    $pesterConfiguration = New-PesterConfiguration -Hashtable $pesterOptions;

    $results = Invoke-Pester -Configuration $pesterConfiguration;

    # Throw an error if pester tests failed
    if ($Null -eq $results) {
        throw 'Failed to get Pester test results.';
    }
    elseif ($results.FailedCount -gt 0) {
        throw "$($results.FailedCount) tests failed.";
    }
}

# Synopsis: Run validation
task Rules Dependencies, {
    $assertParams = @{
        Path         = './.ps-rule/'
        Style        = $AssertStyle
        OutputFormat = 'NUnit3'
        ErrorAction  = 'Stop'
        As           = 'Summary'
        Outcome      = 'Problem'
    }
    Import-Module (Join-Path -Path $PWD -ChildPath out/modules/PSRule.Rules.GitHub) -Force;
    Assert-PSRule @assertParams -InputPath $PWD -Module PSRule.Rules.MSFT.OSS -Format File -OutputPath reports/ps-rule-file.xml;

    $rules = Get-PSRule -Module PSRule.Rules.GitHub;
    $rules | Assert-PSRule @assertParams -OutputPath reports/ps-rule-file2.xml;
}

# Synopsis: Run script analyzer
task Analyze Build, Dependencies, {
    Invoke-ScriptAnalyzer -Path out/modules/PSRule.Rules.GitHub;
}

# Synopsis: Build table of content for rules
task BuildRuleDocs Build, Dependencies, {
    Import-Module (Join-Path -Path $PWD -ChildPath out/modules/PSRule.Rules.GitHub) -Force;
    $Null = Invoke-PSDocument -Name module -OutputPath .\docs\en\rules\ -Path .\RuleToc.Doc.ps1;
    $Null = Invoke-PSDocument -Name resource -OutputPath .\docs\en\rules\ -Path .\RuleToc.Doc.ps1;
}

# Synopsis: Build help
task BuildHelp BuildModule, Dependencies, {
    if (!(Test-Path out/modules/PSRule.Rules.GitHub/en/)) {
        $Null = New-Item -Path out/modules/PSRule.Rules.GitHub/en/ -ItemType Directory -Force;
    }
    if (!(Test-Path out/modules/PSRule.Rules.GitHub/en-US/)) {
        $Null = New-Item -Path out/modules/PSRule.Rules.GitHub/en-US/ -ItemType Directory -Force;
    }
    if (!(Test-Path out/modules/PSRule.Rules.GitHub/en-AU/)) {
        $Null = New-Item -Path out/modules/PSRule.Rules.GitHub/en-AU/ -ItemType Directory -Force;
    }
    if (!(Test-Path out/modules/PSRule.Rules.GitHub/en-GB/)) {
        $Null = New-Item -Path out/modules/PSRule.Rules.GitHub/en-GB/ -ItemType Directory -Force;
    }

    $Null = Copy-Item -Path docs/en/rules/*.md -Destination out/modules/PSRule.Rules.GitHub/en/;

    # Avoid YamlDotNet issue in same app domain
    exec {
        $pwshPath = (Get-Process -Id $PID).Path;
        &$pwshPath -Command {
            # Generate MAML and about topics
            Import-Module -Name PlatyPS -Verbose:$False;
            $Null = New-ExternalHelp -OutputPath 'out/docs/PSRule.Rules.GitHub' -Path '.\docs\commands\PSRule.Rules.GitHub\en-US' -Force;
            # '.\docs\concepts\PSRule.Rules.GitHub\en-US'

             # Copy generated help into module out path
            $Null = Copy-Item -Path out/docs/PSRule.Rules.GitHub/* -Destination out/modules/PSRule.Rules.GitHub/en-US/ -Recurse;
            $Null = Copy-Item -Path out/docs/PSRule.Rules.GitHub/* -Destination out/modules/PSRule.Rules.GitHub/en-AU/ -Recurse;
            $Null = Copy-Item -Path out/docs/PSRule.Rules.GitHub/* -Destination out/modules/PSRule.Rules.GitHub/en-GB/ -Recurse;
        }
    }

    if (!(Test-Path -Path 'out/docs/PSRule.Rules.GitHub/PSRule.Rules.GitHub-help.xml')) {
        throw 'Failed find generated cmdlet help.';
    }
}

task ScaffoldHelp Build, BuildRuleDocs, {
    Import-Module (Join-Path -Path $PWD -ChildPath out/modules/PSRule.Rules.GitHub) -Force;
    Update-MarkdownHelp -Path '.\docs\commands\PSRule.Rules.GitHub\en-US';
}

task Dependencies NuGet, {
    Import-Module $PWD/scripts/dependencies.psm1;
    Install-Dependencies -Path $PWD/modules.json;
}

# Synopsis: Remove temp files.
task Clean {
    Remove-Item -Path out,reports -Recurse -Force -ErrorAction SilentlyContinue;
}

task Build Clean, BuildModule, VersionModule, BuildHelp

task Test Build, Rules, TestDotNet, TestModule

task Release ReleaseModule

# Synopsis: Build and test. Entry point for CI Build stage
task . Build, Rules, TestDotNet
