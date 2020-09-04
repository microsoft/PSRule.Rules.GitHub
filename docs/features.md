# PSRule for GitHub features

The following sections describe key features of PSRule for GitHub.

- [Ready to go](#ready-to-go)
- [DevOps](#devops)
- [Cross-platform](#cross-platform)

## Ready to go

PSRule for GitHub includes pre-build rules for validating public and private repositories.
Each rule includes additional information to help remediate issues.

Use the built-in rules to start enforcing release processes quickly.
Then layer on your own rules as your organization's requirements mature.
Custom rules can be implemented quickly and work side-by-side with built-in rules.

As new built-in rules are added and improved, download the latest PowerShell module to start using them.

## DevOps

GitHub repositories can be validated throughout their lifecycle to support a DevOps culture.

- **Shift-left:** Identify configuration issues and provide fast feedback in pull requests.
- **Monitor continuously:** Perform ongoing checks for configuration optimization opportunities.

PSRule for GitHub provides the following cmdlets that extract data for analysis:

- [Export-GitHubRuleData](docs/commands/PSRule.Rules.GitHub/en-US/Export-GitHubRuleData.md) - Export GitHub repository configuration.

## Cross-platform

PSRule uses modern PowerShell libraries at its core, allowing it to go anywhere PowerShell can go.
The companion extension for Visual Studio Code provides snippets for authoring rules and documentation.
PSRule runs on MacOS, Linux and Windows.

PowerShell makes it easy to integrate PSRule into populate CI systems.
Additionally, PSRule has extensions for:

- [Azure Pipeline (Azure DevOps)][extension-pipelines]
- [GitHub Actions (GitHub)][extension-github]

PSRule for GitHub (`PSRule.Rules.GitHub`) can be installed locally using `Install-Module` within PowerShell.
For additional installation options see [install instructions](install-instructions.md).

## Frequently Asked Questions (FAQ)

Continue reading for FAQ relating to _PSRule for GitHub_.
For general FAQ see [PSRule - Frequently Asked Questions (FAQ)][ps-rule-faq], including:

- [How is PSRule different to Pester?][compare-pester]
- [How do I configure PSRule?][ps-rule-configure]
- [How do I ignore a rule?][ignore-rule]

### What permissions do I need to export data?

**More to come**

### Traditional unit testing vs PSRule for GitHub?

You may already be using a unit test framework such as Pester to test infrastructure code.
If you are, then you may have encountered the following challenges.

For a general PSRule/ Pester comparison see [How is PSRule different to Pester?][compare-pester]

[compare-pester]: https://github.com/microsoft/PSRule/blob/main/docs/features.md#how-is-psrule-different-to-pester
[ignore-rule]: https://github.com/microsoft/PSRule/blob/main/docs/features.md#how-do-i-ignore-a-rule
[ps-rule-configure]: https://github.com/microsoft/PSRule/blob/main/docs/features.md#how-do-i-configure-psrule
[ps-rule-faq]: https://github.com/microsoft/PSRule/blob/main/docs/features.md#frequently-asked-questions-faq
[extension-pipelines]: https://marketplace.visualstudio.com/items?itemName=bewhite.ps-rule
[extension-github]: https://github.com/marketplace/actions/psrule
