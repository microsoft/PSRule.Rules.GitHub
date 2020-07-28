# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#
# Unit tests for GitHub branch rules
#

[CmdletBinding()]
param ()

# Setup error handling
$ErrorActionPreference = 'Stop';
Set-StrictMode -Version latest;

if ($Env:SYSTEM_DEBUG -eq 'true') {
    $VerbosePreference = 'Continue';
}

# Setup tests paths
$rootPath = $PWD;
Import-Module (Join-Path -Path $rootPath -ChildPath out/modules/PSRule.Rules.GitHub) -Force;
$here = (Resolve-Path $PSScriptRoot).Path;

Describe 'GitHub.Branch' -Tag 'Branch' {
    $dataPath = Join-Path -Path $here -ChildPath 'Resources.Repo.1.json';

    Context 'Conditions' {
        $invokeParams = @{
            Module = 'PSRule.Rules.GitHub'
            WarningAction = 'Ignore'
            ErrorAction = 'Stop'
        }

        It 'GitHub.Branch.Name' {
            $option = @{
                'Configuration.GitHub_BranchName_Format' = '^(main|gh-pages|users\/\w*\/\w+|shared\/\w+)$'
            }
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Branch.Name' -Option $option;

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 2;
            $ruleResult.TargetName | Should -BeIn 'master', 'hotfix-description';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 4;
            $ruleResult.TargetName | Should -BeIn 'main', 'gh-pages', 'users/username/description', 'shared/description';
        }
    }

    Context 'Branch names' {
        $invokeParams = @{
            Baseline = 'GitHub'
            Module = 'PSRule.Rules.GitHub'
            WarningAction = 'Ignore'
            ErrorAction = 'Stop'
        }
        $validNames = @(
            'main'
            'gh-pages'
            'hotfix-description'
            'users/username/description'
        )
        $invalidNames = @(
            'master'
        )
        $testObject = [PSCustomObject]@{
            Name = ''
            Type = 'api.github.com/repos/branches'
        }

        # Pass
        foreach ($name in $validNames) {
            It $name {
                $testObject.Name = $name;
                $ruleResult = $testObject | Invoke-PSRule @invokeParams -Name 'GitHub.Branch.Name';
                $ruleResult | Should -Not -BeNullOrEmpty;
                $ruleResult.Outcome | Should -Be 'Pass';
            }
        }

        # Fail
        foreach ($name in $invalidNames) {
            It $name {
                $testObject.Name = $name;
                $ruleResult = $testObject | Invoke-PSRule @invokeParams -Name 'GitHub.Branch.Name';
                $ruleResult | Should -Not -BeNullOrEmpty;
                $ruleResult.Outcome | Should -Be 'Fail';
            }
        }
    }
}
