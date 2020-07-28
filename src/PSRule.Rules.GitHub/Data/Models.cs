// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace PSRule.Rules.GitHub.Data
{
    public sealed class Repository
    {
        private const string OBJECT_TYPE = "api.github.com/repos";

        internal Repository(string owner, string name)
        {
            Owner = owner;
            Name = name;
            FullName = string.Concat(owner, '/', name);
        }

        public string Type => OBJECT_TYPE;

        public string Owner { get; }

        public string Name { get; }

        public string FullName { get; }

        public string RepositoryName => FullName;

        public string Description { get; internal set; }

        public bool Private { get; internal set; }

        public bool Fork { get; internal set; }

        public bool Archived { get; internal set; }

        public string DefaultBranch { get; internal set; }

        public string License { get; internal set; }

        public string HtmlUrl { get; internal set; }

        public string Homepage { get; internal set; }

        public string Language { get; internal set; }

        public bool? AllowRebaseMerge { get; internal set; }

        public bool? AllowSquashMerge { get; internal set; }

        public bool? AllowMergeCommit { get; internal set; }

        public bool HasIssues { get; internal set; }

        public bool HasWiki { get; internal set; }

        public bool HasDownloads { get; internal set; }

        public bool HasPages { get; internal set; }

        public CommunityProfile CommunityProfile { get; internal set; }

        public IEnumerable<string> CommunityFiles { get; internal set; }
    }

    public sealed class Branch
    {
        private const string OBJECT_TYPE = "api.github.com/repos/branches";

        internal Branch(string repositoryName, string name)
        {
            Name = name;
            RepositoryName = repositoryName;
        }

        public string Type => OBJECT_TYPE;

        public string Name { get; }

        public string BranchName => Name;

        public string RepositoryName { get; }

        public BranchProtection Protection { get; internal set; }

        public IEnumerable<BranchStatus> Status { get; internal set; }
    }

    public sealed class BranchProtection
    {

        internal BranchProtection(bool enabled)
        {
            Enabled = enabled;
        }

        public bool Enabled { get; }

        public bool? EnforceAdmins { get; internal set; }

        public bool? RequireUpToDate { get; internal set; }

        public string[] RequireStatusChecks { get; internal set; }
    }

    public sealed class BranchStatus
    {
        internal BranchStatus(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Status { get; internal set; }

        public string Conclusion { get; internal set; }
    }

    public sealed class Label
    {
        internal Label(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Description { get; internal set; }

        public string Color { get; internal set; }

        public bool Default { get; internal set; }
    }

    public sealed class Milestone
    {
        internal Milestone(int number)
        {
            Number = number;
        }

        public int Number { get; }

        public string Title { get; internal set; }

        public string Description { get; internal set; }

        public string State { get; internal set; }
    }

    public sealed class Release
    {
        internal Release(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Prerelease { get; internal set; }

        public string TagName { get; internal set; }
    }

    public sealed class RepositoryTag
    {
        internal RepositoryTag(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class CommunityProfile
    {
        public bool CodeOfConduct { get; set; }

        public bool Contributing { get; set; }

        public bool IssueTemplate { get; set; }

        public bool PullRequestTemplate { get; set; }

        public bool License { get; set; }

        public bool ReadMe { get; set; }
    }
}
