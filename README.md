# PSRule for GitHub

A suite of rules to validate GitHub repositories using PSRule.

![ci-badge]

**More to come soon.**

## Disclaimer

This project is open source and **not a supported product**.

If you are experiencing problems, have a feature request, or a question, please check for an [issue] on GitHub.
If you do not see your problem captured, please file a new issue, and follow the provided template.

If you have any problems with the [PSRule][engine] engine, please check the project GitHub [issues](https://github.com/Microsoft/PSRule/issues) page instead.

## Getting started

### Export repository

To validate a GitHub repository, first export configuration data with the `Export-GitHubRuleData` cmdlet.

For example:

```powershell
# Export repository configuration data for Microsoft/PSRule
Export-GitHubRuleData -Repository 'Microsoft/PSRule';
```

To export multiple repositories:

- Comma separate each repository.
- Use `<organization>/` to include all repositories in the organization.

Authenticate to export private repositories by:

- Using `-Credential` to specify a `PSCredential` object with a personal access token (PAT).
The username of `PSCredential` is ignored.
- Using `-UseGitHubToken` to read a PAT token from the `GITHUB_TOKEN` environment variable.

### Validate repositories

To validate a GitHub repository using the extracted data use the `Assert-PSRule` cmdlet.

For example:

```powershell
Assert-PSRule -InputPath .\*.json -Module 'PSRule.Rules.GitHub';
```

For advanced usage, see [Assert-PSRule](https://microsoft.github.io/PSRule/commands/PSRule/en-US/Assert-PSRule.html) help.

## Rule reference

For a list of rules included in the `PSRule.Rules.GitHub` module see:

- [Rules by category](docs/rules/en/module.md)

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

[issue]: https://github.com/Microsoft/PSRule.Rules.GitHub/issues
[install]: docs/scenarios/install-instructions.md
[ci-badge]: https://dev.azure.com/bewhite/PSRule.Rules.GitHub/_apis/build/status/PSRule.Rules.GitHub-CI?branchName=main
[module]: https://www.powershellgallery.com/packages/PSRule.Rules.GitHub
[engine]: https://github.com/Microsoft/PSRule
