# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Synopsis: Branch uses allowed branch names.
Rule 'GitHub.Branch.Name' -Type 'api.github.com/repos/branches' {
    $avoidNames = @(
        'master'
    )
    $Assert.NotIn($TargetObject, 'Name', $avoidNames);
}
