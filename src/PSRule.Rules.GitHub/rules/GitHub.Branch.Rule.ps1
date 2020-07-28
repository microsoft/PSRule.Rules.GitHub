# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Synopsis: Branch uses allowed branch names.
Rule 'GitHub.Branch.Name' -Type 'api.github.com/repos/branches' {
    $Assert.NotIn($TargetObject, 'Name', $Configuration.GitHub_BranchName_Disallowed);
    $Assert.Match($TargetObject, 'Name', $Configuration.GitHub_BranchName_Format);
}
