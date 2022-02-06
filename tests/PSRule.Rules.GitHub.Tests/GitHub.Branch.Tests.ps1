# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#
# Unit tests for GitHub branch rules
#

[CmdletBinding()]
param ()

BeforeAll {
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
}

Describe 'GitHub.Branch' -Tag 'Branch' {
    BeforeAll {
        $dataPath = Join-Path -Path $here -ChildPath 'Resources.Repo.1.json';
    }

    Context 'Conditions' {
        BeforeAll {
            $invokeParams = @{
                Module = 'PSRule.Rules.GitHub'
                WarningAction = 'Ignore'
                ErrorAction = 'Stop'
            }
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
        BeforeAll {
            $invokeParams = @{
                Baseline = 'GitHub'
                Module = 'PSRule.Rules.GitHub'
                WarningAction = 'Ignore'
                ErrorAction = 'Stop'
            }
            $testObject = [PSCustomObject]@{
                Name = ''
                Type = 'api.github.com/repos/branches'
            }
        }

        BeforeDiscovery {
            $validNames = @(
                'main'
                'gh-pages'
                'hotfix-description'
                'users/username/description'
            )
            $invalidNames = @(
                'master'
            )
        }

        # Pass
        It '<_>' -ForEach $validNames {
            $testObject.Name = $_;
            $ruleResult = $testObject | Invoke-PSRule @invokeParams -Name 'GitHub.Branch.Name';
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Outcome | Should -Be 'Pass';
        }

        # Fail
        It '<_>' -ForEach $invalidNames {
            $testObject.Name = $_;
            $ruleResult = $testObject | Invoke-PSRule @invokeParams -Name 'GitHub.Branch.Name';
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Outcome | Should -Be 'Fail';
        }
    }
}
