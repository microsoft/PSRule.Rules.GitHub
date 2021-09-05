// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Rules.GitHub.Configuration;
using PSRule.Rules.GitHub.Pipeline;
using System;
using System.Management.Automation;
using System.Security;

namespace PSRule.Rules.GitHub.Runtime
{
    /// <summary>
    /// External helper to be referenced within rules.
    /// </summary>
    public static class Helper
    {
        private const string GITHUB_REPOSITORY = "GITHUB_REPOSITORY";
        private const string GITHUB_TOKEN = "GITHUB_TOKEN";

        public static PSObject[] GetRepository()
        {
            var repos = PSRuleOption.TryGetEnvironmentVariableString(GITHUB_REPOSITORY, out string repo) ? new string[] { repo } : null;
            var credential = PSRuleOption.TryGetEnvironmentVariableSecureString(GITHUB_TOKEN, out SecureString token) ? new PSCredential("token", token) : null;
            if (repos == null || repos.Length == 0 || credential == null)
                return Array.Empty<PSObject>();

            var context = new GitHubContext(repos, credential);
            var helper = new RepositoryHelper(context);
            return helper.Get(repo);
        }
    }
}
