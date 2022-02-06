# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#
# Conventions definitions
#

# Synopsis: Expand the current GitHub repository.
Export-PSRuleConvention 'GitHub.ExpandRepository' -If { ![String]::IsNullOrEmpty($Env:GITHUB_REPOSITORY) -and $TargetObject -is [PSRule.Data.RepositoryInfo] } -Begin {
    try {
        $PSRule.Import([PSRule.Rules.GitHub.Runtime.Helper]::GetRepository());
    }
    catch {
        Write-Error $_
    }
}
