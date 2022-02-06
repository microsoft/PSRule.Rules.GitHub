# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#
# Unit tests for GitHub repository rules
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

Describe 'GitHub.Repo' -Tag 'Repository' {
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

        It 'GitHub.Repo.Protected' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.Protected';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.CodeOfConduct' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.CodeOfConduct';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.Contributing' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.Contributing';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.Readme' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.Readme';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.CodeOwners' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.CodeOwners';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.License' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.License';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.Description' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.Description';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.IssueTempate' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.IssueTempate';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }

        It 'GitHub.Repo.PRTemplate' {
            $filteredResult = Invoke-PSRule @invokeParams -InputPath $dataPath -Name 'GitHub.Repo.PRTemplate';

            # Fail
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Fail' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-B';

            # Pass
            $ruleResult = @($filteredResult | Where-Object { $_.Outcome -eq 'Pass' });
            $ruleResult | Should -Not -BeNullOrEmpty;
            $ruleResult.Length | Should -Be 1;
            $ruleResult.TargetName | Should -BeIn 'org/repository-A';
        }
    }
}
