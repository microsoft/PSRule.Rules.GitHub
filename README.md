# PSRule for GitHub

A suite of rules to validate GitHub repositories using PSRule.

[![Open in vscode.dev](https://img.shields.io/badge/Open%20in-vscode.dev-blue)][1]

Features of PSRule for GitHub include:

- [Ready to go][2] - Leverage pre-built rules.
- [DevOps][3] - Validate repositories throughout their lifecycle.
- [Cross-platform][4] - Run with GitHub Actions or other CI integrations.

  [1]: https://vscode.dev/github/microsoft/PSRule.Rules.GitHub
  [2]: docs/features.md#ready-to-go
  [3]: docs/features.md#devops
  [4]: docs/features.md#cross-platform

## Support

This project uses GitHub Issues to track bugs and feature requests.
Please search the existing issues before filing new issues to avoid duplicates.

- For new issues, file your bug or feature request as a new [issue].
- For help, discussion, and support questions about using this project, join or start a [discussion].

If you have any problems with the [PSRule][engine] engine, please check the project GitHub [issues](https://github.com/microsoft/PSRule/issues) page instead.

Support for this project/ product is limited to the resources listed above.

## Getting the modules

This project requires the `PSRule` PowerShell module. For details on each see [install].

You can download and install these modules from the PowerShell Gallery.

Module              | Description | Downloads / instructions
------              | ----------- | ------------------------
PSRule.Rules.GitHub | Validate GitHub repositories using PSRule. | [latest][module] / [instructions][install]

## Getting started

### Using with GitHub Actions

The following example shows how to setup GitHub Actions to validate GitHub repositories.

1. See [Creating a workflow file][create-workflow].
2. Reference `microsoft/ps-rule` with `modules: 'PSRule.Rules.GitHub'`.

For example:

```yaml
# Example: .github/workflows/analyze-gh.yaml

#
# STEP 1: Repository validation
#
name: Analyze repository
on:
- push
jobs:
  analyze_arm:
    name: Analyze repository
    runs-on: ubuntu-latest
    steps:

    - name: Checkout
      uses: actions/checkout@v3

    # STEP 2: Run analysis against exported data
    - name: Analyze repository
      uses: microsoft/ps-rule@v2.2.0
      with:
        modules: 'PSRule.Rules.GitHub'
```

### Using locally

The following example shows how to setup PSRule locally to validate templates pre-flight.

1. Install the `PSRule.Rules.GitHub` module and dependencies from the PowerShell Gallery.
2. Export repository data for analysis.
3. Run analysis against a GitHub repository.

For example:

```powershell
# STEP 1: Install PSRule.Rules.GitHub from the PowerShell Gallery
Install-Module -Name 'PSRule.Rules.GitHub' -Scope CurrentUser;

# STEP 2: Export repository configuration data for microsoft/PSRule
Export-GitHubRuleData -Repository 'microsoft/PSRule';

# STEP 3: Run analysis against exported data
Assert-PSRule -Module 'PSRule.Rules.GitHub' -InputPath './*.json';
```

The `Export-GitHubRuleData` cmdlet exports repository data to JSON.
To export multiple repositories:

- Comma separate each repository.
- Use `<organization>/` to include all repositories in the organization.

Authenticate to export private repositories by:

- Using `-Credential` to specify a `PSCredential` object with a personal access token (PAT).
The username of `PSCredential` is ignored.
- Using `-UseGitHubToken` to read a PAT token from the `GITHUB_TOKEN` environment variable.

For advanced usage, see [Assert-PSRule](https://microsoft.github.io/PSRule/v2/commands/PSRule/en-US/Assert-PSRule/) help.

## Rule reference

For a list of rules included in the `PSRule.Rules.GitHub` module see:

- [Rules by category](docs/en/rules/module.md)

## Language reference

PSRule for GitHub extends PowerShell with the following features.

### Commands

The following commands exist in the `PSRule.Rules.GitHub` module:

- [Export-GitHubRuleData](docs/commands/PSRule.Rules.GitHub/en-US/Export-GitHubRuleData.md) - Export GitHub repository configuration.

## Changes and versioning

Modules in this repository will use the [semantic versioning](http://semver.org/) model to declare breaking changes from v1.0.0.
Prior to v1.0.0, breaking changes may be introduced in minor (0.x.0) version increments.
For a list of module changes please see the [change log](CHANGELOG.md).

> Pre-release module versions are created on major commits and can be installed from the PowerShell Gallery.
> Pre-release versions should be considered experimental.
> Modules and change log details for pre-releases will be removed as standard releases are made available.

## Contributing

This project welcomes contributions and suggestions.
If you are ready to contribute, please visit the [contribution guide](CONTRIBUTING.md).

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Maintainers

- [Bernie White](https://github.com/BernieWhite)

## License

This project is [licensed under the MIT License](LICENSE).

[issue]: https://github.com/microsoft/PSRule.Rules.GitHub/issues
[discussion]: https://github.com/microsoft/PSRule.Rules.GitHub/discussions
[install]: docs/install-instructions.md
[module]: https://www.powershellgallery.com/packages/PSRule.Rules.GitHub
[engine]: https://github.com/microsoft/PSRule
