---
category: Branch
severity: Awareness
online version: https://github.com/BernieWhite/PSRule.Rules.GitHub/blob/main/docs/rules/en/GitHub.Branch.Name.md
---

# Use consistent branch names

## SYNOPSIS

Use a consistent naming convention for your branches to identify the work done in the branch.

## DESCRIPTION

A branch is a human readable movable pointer to a commit.
A consistent naming convention help to:

- Identity work across a team or individuals.
- Integrate with continuous integration (CI) systems.

Some suggestions for naming your branches:

- users/username/description
- users/username/workitem
- bugfix/description
- features/feature-name
- hotfix/description
- shared/description

Avoid using branch names such as `master`.

## RECOMMENDATION

Consider using an consistent naming convention for your branches.
Avoid using branch names that may cause offence or exclude members in the community.

## NOTES

To configure this rule:

- Override the `GitHub_BranchName_Format` to set the allow branch format as a regular expression.
Any branch name format is allowed by default.
- Override the `GitHub_BranchName_Disallowed` to set disallowed branch names.

## LINKS

- [Internet Engineering Task Force - Terminology, Power and Oppressive Language](https://tools.ietf.org/id/draft-knodel-terminology-00.html)
- [Renaming the default branch from master](https://github.com/github/renaming/)
