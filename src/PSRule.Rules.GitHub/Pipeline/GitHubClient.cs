// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Rules.GitHub.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PSRule.Rules.GitHub.Pipeline
{
    internal sealed class GitHubClient
    {
        private readonly string[] GITHUB_HEADERS_COMMUNITY_PROFILE = new string[] { "application/vnd.github.v3+json", "application/vnd.github.black-panther+json" };

        private readonly Octokit.GitHubClient _Client;
        private readonly HttpClient _HttpClient;

        public GitHubClient(GitHubContext serviceContext)
        {
            _Client = serviceContext.GetClient();
            _HttpClient = serviceContext.GetHttpClient();
        }

        public Branch[] GetBranches(string owner, string name)
        {
            var items = GetBranchesInternal(owner, name);
            var results = new Data.Branch[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                var protection = items[i].Protected ? GetBranchProtectionInternal(owner, name, items[i].Name) : null;
                var status = GetBranchStatus(owner, name, items[i].Name);
                results[i] = new Branch(string.Concat(owner, '/', name), items[i].Name)
                {
                    Protection = new BranchProtection(items[i].Protected)
                    {
                        EnforceAdmins = protection?.EnforceAdmins?.Enabled,
                        RequireUpToDate = protection?.RequiredStatusChecks?.Strict,
                        RequireStatusChecks = protection?.RequiredStatusChecks?.Contexts?.ToArray(),
                    },
                    Status = status,
                };
            }
            return results;
        }

        private IEnumerable<BranchStatus> GetBranchStatus(string owner, string name, string branch)
        {
            var result = new List<BranchStatus>();
            var statuses = GetBranchStatusInternal(owner, name, branch);
            for (var i = 0; i < statuses.Count; i++)
            {
                result.Add(new BranchStatus(statuses[i].Name)
                {
                    Status = statuses[i].Status.StringValue,
                    Conclusion = statuses[i].Conclusion?.StringValue,
                });
            }
            return result;
        }

        /// <summary>
        /// Get matching repositories for the GitHub organization.
        /// </summary>
        public Octokit.Repository[] GetRepository(string repositorySlug)
        {
            var slugParts = repositorySlug.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var owner = slugParts[0];
            if (slugParts.Length == 2)
                return GetSingleRepository(owner, slugParts[1]);

            var isOrg = IsOrg(owner);
            return isOrg ? GetOrgRepository(owner) : GetUserRepository(owner);
        }

        public Data.CommunityProfile GetCommunityProfile(string owner, string name)
        {
            var profile = _HttpClient.Get<Data.CommunityProfile>($"https://api.github.com/repos/{owner}/{name}/community/profile", headers: GITHUB_HEADERS_COMMUNITY_PROFILE);
            return profile;
        }

        /// <summary>
        /// Get branches for the repository.
        /// </summary>
        private Octokit.Branch[] GetBranchesInternal(string owner, string name)
        {
            var task = _Client.Repository.Branch.GetAll(owner, name);
            task.Wait();
            return task.Result.ToArray();
        }

        private Octokit.BranchProtectionSettings GetBranchProtectionInternal(string owner, string name, string branch)
        {
            try
            {
                var task = _Client.Repository.Branch.GetBranchProtection(owner, name, branch);
                task.Wait();
                return task.Result;
            }
            catch (AggregateException e)
            {
                var baseException = e.GetBaseException();
                if (baseException is Octokit.NotFoundException)
                    return null;

                throw;
            }
        }

        private IReadOnlyList<Octokit.CheckRun> GetBranchStatusInternal(string owner, string name, string branch)
        {
            try
            {
                var task = _Client.Check.Run.GetAllForReference(owner, name, branch);
                task.Wait();
                return task.Result.CheckRuns;
            }
            catch (AggregateException e)
            {
                var baseException = e.GetBaseException();
                if (baseException is Octokit.NotFoundException)
                    return null;

                throw;
            }
        }

        /// <summary>
        /// Get issue milestones for the repository.
        /// </summary>
        public Octokit.Milestone[] GetMilestones(string owner, string name)
        {
            var task = _Client.Issue.Milestone.GetAllForRepository(owner, name);
            task.Wait();
            return task.Result.ToArray();
        }

        /// <summary>
        /// Get releases for the repository.
        /// </summary>
        public Octokit.Release[] GetReleases(string owner, string name)
        {
            var task = _Client.Repository.Release.GetAll(owner, name);
            task.Wait();
            return task.Result.ToArray();
        }

        /// <summary>
        /// Get issue labels for the repository.
        /// </summary>
        public Octokit.Label[] GetLabels(string owner, string name)
        {
            var task = _Client.Issue.Labels.GetAllForRepository(owner, name);
            task.Wait();
            return task.Result.ToArray();
        }

        public Octokit.RepositoryTag[] GetTags(string owner, string name)
        {
            var task = _Client.Repository.GetAllTags(owner, name);
            task.Wait();
            return task.Result.ToArray();
        }

        private static readonly string[] GitHubPaths = new string[]
        {
            ".github",
            ".github/ISSUE_TEMPLATE",
            ".github/PULL_REQUEST_TEMPLATE",
            "docs",
            "docs/PULL_REQUEST_TEMPLATE",
            "PULL_REQUEST_TEMPLATE"
        };

        public string[] GetCommunityFiles(string owner, string name)
        {
            var contentFiles = GetGitHubFiles(owner, name, GitHubPaths);
            var files = new List<string>();
            for (var i = 0; i < contentFiles.Length; i++)
                IncludeCommunityFile(contentFiles[i], files);

            return files.ToArray();
        }

        private Octokit.RepositoryContent[] GetGitHubFiles(string owner, string name, string[] paths)
        {
            var tasks = new Task<IReadOnlyList<Octokit.RepositoryContent>>[paths.Length + 1];
            tasks[0] = _Client.Repository.Content.GetAllContents(owner, name);
            for (var i = 0; i < paths.Length; i++)
                tasks[i + 1] = _Client.Repository.Content.GetAllContents(owner, name, paths[i]);

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException)
            {
                // Discard AggregateExceptions for tasks
            }
            var result = new List<Octokit.RepositoryContent>();
            for (var i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsFaulted)
                    result.AddRange(tasks[i].Result);
            }
            return result.ToArray();
        }

        private static void IncludeCommunityFile(Octokit.RepositoryContent content, List<string> files)
        {
            if (content.Type.Value == Octokit.ContentType.File && IsCommunityFile(content.Path))
                files.Add(content.Path);
        }

        private static bool IsCommunityFile(string path)
        {
            return path.StartsWith(".github/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("README", StringComparison.OrdinalIgnoreCase) || path.StartsWith("LICENSE", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("CODE_OF_CONDUCT.md", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("CONTRIBUTING.md", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("SECURITY.md", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("SUPPORT.md", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("pull_request_template.md", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("pull_request_template.txt", StringComparison.OrdinalIgnoreCase);
        }

        private Octokit.Repository[] GetUserRepository(string user)
        {
            var task = _Client.Repository.GetAllForUser(user);
            task.Wait();
            return task.Result.ToArray();
        }

        private Octokit.Repository[] GetOrgRepository(string org)
        {
            var task = _Client.Repository.GetAllForOrg(org);
            task.Wait();
            return task.Result.ToArray();
        }

        /// <summary>
        /// Do a lookup against a login to determine if it is an organization.
        /// </summary>
        private bool IsOrg(string login)
        {
            var task = _Client.User.Get(login).ContinueWith(u => u.Result.Type == Octokit.AccountType.Organization);
            task.Wait();
            return task.Result;
        }

        private Octokit.Repository[] GetSingleRepository(string owner, string name)
        {
            var task = _Client.Repository.Get(owner, name).ContinueWith(r => r.Result);
            task.Wait();
            return new Octokit.Repository[] { task.Result };
        }
    }
}
