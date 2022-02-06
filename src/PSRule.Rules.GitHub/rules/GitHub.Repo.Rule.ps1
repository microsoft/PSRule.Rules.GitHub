# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Synopsis: Default branch is protected.
Rule 'GitHub.Repo.Protected' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $defaultBranch = @(GetRepositoryBranch -Name $TargetObject.DefaultBranch);
    $Assert.HasFieldValue($defaultBranch[0], 'Protection.Enabled', $True);
}

# Synopsis: Use CODE_OF_CONDUCT.md file in default branch.
Rule 'GitHub.Repo.CodeOfConduct' -Type 'api.github.com/repos' -If { !$TargetObject.Private -and !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('CODE_OF_CONDUCT.md', '.github/CODE_OF_CONDUCT.md', 'docs/CODE_OF_CONDUCT.md')).
        Reason($LocalizedData.RepositoryFileNotExist, 'CODE_OF_CONDUCT.md');
}

# Synopsis: Use CONTRIBUTING.md file in default branch.
Rule 'GitHub.Repo.Contributing' -Type 'api.github.com/repos' -If { !$TargetObject.Private -and !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('CONTRIBUTING.md', '.github/CONTRIBUTING.md', 'docs/CONTRIBUTING.md')).
        Reason($LocalizedData.RepositoryFileNotExist, 'CONTRIBUTING.md');
}

# Synopsis: Use README.md file in default branch.
Rule 'GitHub.Repo.Readme' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('README.md')).
        Reason($LocalizedData.RepositoryFileNotExist, 'README.md');
}

# Synopsis: Use CODEOWNERS file in default branch.
Rule 'GitHub.Repo.CodeOwners' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('CODEOWNERS', '.github/CODEOWNERS', 'docs/CODEOWNERS')).
        Reason($LocalizedData.RepositoryFileNotExist, 'CODEOWNERS');
}

# Synopsis: Use LICENSE file in default branch.
Rule 'GitHub.Repo.License' -Type 'api.github.com/repos' -If { !$TargetObject.Private -and !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @('LICENSE', 'LICENSE.txt', 'LICENSE.md', 'LICENSE.rst')).
        Reason($LocalizedData.RepositoryFileNotExist, 'LICENSE');
}

# Synopsis: Define a description for the repository.
Rule 'GitHub.Repo.Description' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $Assert.HasFieldValue($TargetObject, 'description');
}

# Synopsis: Use one or more issue templates.
Rule 'GitHub.Repo.IssueTempate' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $issueTemplates = @($TargetObject.CommunityFiles | Where-Object {
        $_ -like ".github/ISSUE_TEMPLATE/*"
    });
    $Assert.
        GreaterOrEqual($issueTemplates, '.', 1).
        Reason($LocalizedData.IssueTemplates);
}

# Synopsis: Use a .github/PULL_REQUEST_TEMPLATE.md file in default branch.
Rule 'GitHub.Repo.PRTemplate' -Type 'api.github.com/repos' -If { !$TargetObject.Fork } {
    $Assert.
        In($TargetObject, 'CommunityFiles', @(
            'pull_request_template.md',
            '.github/pull_request_template.md',
            '.github/PULL_REQUEST_TEMPLATE/pull_request_template.md',
            'docs/pull_request_template.md'
        )).
        Reason($LocalizedData.RepositoryFileNotExist, '.github/pull_request_template.md');
}
