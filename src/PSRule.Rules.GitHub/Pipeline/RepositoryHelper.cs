// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Management.Automation;

namespace PSRule.Rules.GitHub.Pipeline;

internal sealed class RepositoryHelper
{
    private const string PROPERTY_BRANCHES = "Branches";
    private const string PROPERTY_LABELS = "Labels";
    private const string PROPERTY_MILESTONES = "Milestones";
    private const string PROPERTY_RELEASES = "Releases";
    private const string PROPERTY_TAGS = "Tags";

    private readonly GitHubClient _Client;

    public RepositoryHelper(GitHubContext serviceContext)
    {
        _Client = new GitHubClient(serviceContext);
    }

    public PSObject[] Get(string repositorySlug)
    {
        var result = new List<PSObject>();
        var repos = _Client.GetRepository(repositorySlug);
        for (var r = 0; r < repos.Length; r++)
        {
            var repo = PSObject.AsPSObject(repos[r]);
            var branches = _Client.GetBranches(repos[r].Owner, repos[r].Name);
            var labels = _Client.GetLabels(repos[r].Owner, repos[r].Name);
            var milestones = _Client.GetMilestones(repos[r].Owner, repos[r].Name);
            var releases = _Client.GetReleases(repos[r].Owner, repos[r].Name);
            var tags = _Client.GetTags(repos[r].Owner, repos[r].Name);

            repo.Properties.Add(new PSNoteProperty(PROPERTY_BRANCHES, branches));
            repo.Properties.Add(new PSNoteProperty(PROPERTY_LABELS, labels));
            repo.Properties.Add(new PSNoteProperty(PROPERTY_MILESTONES, milestones));
            repo.Properties.Add(new PSNoteProperty(PROPERTY_RELEASES, releases));
            repo.Properties.Add(new PSNoteProperty(PROPERTY_TAGS, tags));
            result.Add(repo);

            // Write branches as separate objects
            for (var b = 0; b < branches.Length; b++)
            {
                var branch = PSObject.AsPSObject(branches[b]);
                result.Add(branch);
            }
        }
        return result.ToArray();
    }
}
