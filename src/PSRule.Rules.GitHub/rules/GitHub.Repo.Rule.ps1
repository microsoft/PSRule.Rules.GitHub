# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Synopsis: Default branch is protected.
Rule 'GitHub.Repo.Protected' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $defaultBranch = @(GetRepositoryBranch -Name $TargetObject.DefaultBranch);
    $Assert.HasFieldValue($defaultBranch[0], 'Protection.Enabled', $True);
}

# Synopsis: Has recommended community files for public repositories.
Rule 'GitHub.Repo.Community' -Type 'api.github.com/repos' -If { !$TargetObject.Private -and !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('README.md')).
        Reason($LocalizedData.RepositoryFileNotExist, 'README.md');
    $Assert.
        In($TargetObject, 'CommunityFiles', @('CODE_OF_CONDUCT.md', '.github/CODE_OF_CONDUCT.md', 'docs/CODE_OF_CONDUCT.md')).
        Reason($LocalizedData.RepositoryFileNotExist, 'CODE_OF_CONDUCT.md');
    $Assert.
        In($TargetObject, 'CommunityFiles', @('CONTRIBUTING.md', '.github/CONTRIBUTING.md', 'docs/CONTRIBUTING.md')).
        Reason($LocalizedData.RepositoryFileNotExist, 'CONTRIBUTING.md');
}

# Synopsis: Use CODEOWNERS file in default branch.
Rule 'GitHub.Repo.CodeOwners' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('CODEOWNERS', '.github/CODEOWNERS', 'docs/CODEOWNERS')).
        Reason($LocalizedData.RepositoryFileNotExist, 'CODEOWNERS')
}
