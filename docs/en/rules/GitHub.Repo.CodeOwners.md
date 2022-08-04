---
category: Repository
severity: Important
online version: https://github.com/microsoft/PSRule.Rules.GitHub/blob/main/docs/en/rules/GitHub.Repo.CodeOwners.md
---

# Use a CODEOWNERS file

## SYNOPSIS

Use a CODEOWNERS file in the default branch.

## DESCRIPTION

You can use a CODEOWNERS file to define individuals or teams that are responsible for code in a repository.
When a pull request is created, code owners for the base branch are automatically requested for review.

A CODEOWNERS file can be created in the repository root, `.github/`, or `docs/` directories.

## RECOMMENDATION

Consider creating a CODEOWNERS file in the default branch.
Additionally consider creating CODEOWNERS files for each protected branch.

## LINKS

- [Introducing code owners](https://github.blog/2017-07-06-introducing-code-owners/)
- [About code owners](https://docs.github.com/en/github/creating-cloning-and-archiving-repositories/about-code-owners)
