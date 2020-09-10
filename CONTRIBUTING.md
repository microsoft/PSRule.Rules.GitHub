# Contributing to PSRule for GitHub

Welcome, and thank you for your interest in contributing to PSRule!

There are many ways in which you can contribute, beyond writing code.
The goal of this document is to provide a high-level overview of how you can get involved.

- [Reporting issues](#reporting-issues)
- [Improve documentation](#improving-documentation)
- [Adding or improving rules](#adding-or-improving-rules)
- Fix bugs or add features

## Contributor License Agreement (CLA)

This project welcomes contributions and suggestions. Most contributions require you to
agree to a Contributor License Agreement (CLA) declaring that you have the right to,
and actually do, grant us the rights to use your contribution. For details, visit
https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need
to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the
instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Reporting issues

Have you identified a reproducible problem?
Have a feature request?
We want to hear about it!
Here's how you can make reporting your issue as effective as possible.

### Look for an existing issue

Before you create a new issue, please do a search in [open issues][issues] to see if the issue or feature request has already been filed.

If you find your issue already exists,
make relevant comments and add your [reaction](https://github.com/blog/2119-add-reactions-to-pull-requests-issues-and-comments).
Use a reaction in place of a "+1" comment:

* 👍 - upvote

## Improving documentation

This project contains a wide range of documentation, stored in `docs/`.
Some of the documentation that you might like to improve include:

- Rule recommendations (`docs/rules/`).
- Scenarios and examples (`docs/scenarios/`).
- PowerShell cmdlet and conceptual topics (`docs/commands/` and `docs/concepts/`).

### Rule recommendations

Before improving rule recommendations familiarize yourself with writing [rule markdown documentation](https://microsoft.github.io/PSRule/scenarios/rule-docs/rule-docs.html#writing-markdown-documentation).

Rule documentation requires the following annotations:

- `severity`
- `category`
- `online version`

## Adding or improving rules

- Rules are stored in `src/PSRule.Rules.GitHub/rules/`.
- Rule documentation in English is stored in `docs/rules/en/`.
  - Additional cultures can be added in a subdirectory under `docs/rules/`.
- Use pre-conditions to limit the type of resource a rule applies to.

**Tips for authoring rules:**

- To create new rules, snippets in the VS Code extension for PSRule can be used.
- Use `-Type` over `-If` pre-conditions when possible.
Both may be required in some cases.

### Intro to Git and GitHub

When contributing to documentation or code changes, you'll need to have a GitHub account and a basic understanding of Git.
Check out the links below to get started.

- Make sure you have a [GitHub account][github-signup].
- GitHub Help:
  - [Git and GitHub learning resources][learn-git].
  - [GitHub Flow Guide][github-flow].
  - [Fork a repo][github-fork].
  - [About Pull Requests][github-pr].

### Code editor

You should use the multi-platform [Visual Studio Code][vscode] (VS Code).
The project contains a number of workspace specific settings that make it easier to author consistently.

After installing VS Code, install the following extensions:

- [PSRule](https://marketplace.visualstudio.com/items?itemName=bewhite.psrule-vscode-preview)
- [YAML](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml)
- [PowerShell](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell)
- [Code Spell Checker](https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker)

### Building and testing

When creating a pull request to merge your changes, a continuous integration (CI) pipeline is run.
The CI pipeline will build then test your changes across MacOS, Linux and Windows configurations.

Before opening a pull request try building your changes locally.
To do this See [building from source][build] for instructions.

## Thank You!

Your contributions to open source, large or small, make great projects like this possible.
Thank you for taking the time to contribute.

[learn-git]: https://help.github.com/en/articles/git-and-github-learning-resources
[github-flow]: https://guides.github.com/introduction/flow/
[github-signup]: https://github.com/signup/free
[github-fork]: https://help.github.com/en/github/getting-started-with-github/fork-a-repo
[github-pr]: https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests
[github-pr-create]: https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request-from-a-fork
[build]: docs/install-instructions.md#building-from-source
[vscode]: https://code.visualstudio.com/
[issues]: https://github.com/Microsoft/PSRule.Rules.GitHub/issues
