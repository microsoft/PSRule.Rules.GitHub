# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Synopsis: Default branch is protected.
Rule 'GitHub.Repo.Protected' -Type 'api.github.com/repos' {
    $defaultBranch = @(GetRepositoryBranch -Name $TargetObject.DefaultBranch);
    $Assert.HasFieldValue($defaultBranch[0], 'Protection.Enabled', $True);
}

# Synopsis: Has recommended community files for public repositories.
Rule 'GitHub.Repo.Community' -Type 'api.github.com/repos' -If { !$TargetObject.Private -and !$TargetObject.Fork } {
    # $Assert.In($TargetObject, 'CommunityFiles', @('README.md'));
}
