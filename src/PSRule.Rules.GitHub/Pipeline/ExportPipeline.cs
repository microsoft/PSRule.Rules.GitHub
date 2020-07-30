// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Rules.GitHub.Configuration;
using PSRule.Rules.GitHub.Pipeline.Output;
using System;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace PSRule.Rules.GitHub.Pipeline
{
    public interface IExportPipelineBuilder : IPipelineBuilder
    {
        void Repository(string[] repo);

        void Credential(PSCredential credential);
    }

    internal sealed class ExportPipelineBuilder : PipelineBuilderBase, IExportPipelineBuilder
    {
        private const string OUTPUTFILE_PREFIX = "github-";
        private const string OUTPUTFILE_EXTENSION = ".json";

        private const string GITHUB_REPOSITORY = "GITHUB_REPOSITORY";
        private const string GITHUB_TOKEN = "GITHUB_TOKEN";

        private string[] _Repository;
        private bool _UseGitHubToken;
        private PSCredential _Credential;
        private bool _PassThru;

        public ExportPipelineBuilder(PSRuleOption option)
        {
            _PassThru = false;
            Configure(option);
        }

        public void Repository(string[] repository)
        {
            if (repository == null)
            {
                if (TryGetEnvironmentVariableString(GITHUB_REPOSITORY, out string repo))
                    _Repository = new string[] { repo };

                return;
            }
            _Repository = repository;
        }

        public void UseGitHubToken(bool useGitHubToken)
        {
            _UseGitHubToken = useGitHubToken;
        }

        public void Credential(PSCredential credential)
        {
            if (_UseGitHubToken && credential == null)
            {
                if (TryGetEnvironmentVariableSecureString(GITHUB_TOKEN, out SecureString token))
                    _Credential = new PSCredential("token", token);

                return;
            }
            _Credential = credential;
        }

        public void PassThru(bool passThru)
        {
            _PassThru = passThru;
        }

        public override IPipeline Build()
        {
            return new ExportPipeline(PrepareContext(), PrepareWriter(), GetGitHubContext());
        }

        protected override PipelineWriter PrepareWriter()
        {
            return _PassThru ? base.PrepareWriter() : new JsonOutputWriter(GetOutput(), Option);
        }

        protected override PipelineWriter GetOutput()
        {
            // Redirect to file instead
            if (!string.IsNullOrEmpty(Option.Output.Path))
            {
                return new FileOutputWriter(
                    inner: base.GetOutput(),
                    option: Option,
                    encoding: GetEncoding(Option.Output.Encoding),
                    path: Option.Output.Path,
                    defaultFile: string.Concat(OUTPUTFILE_PREFIX, Guid.NewGuid().ToString().Substring(0, 8), OUTPUTFILE_EXTENSION),
                    shouldProcess: CmdletContext.ShouldProcess
                );
            }
            return base.GetOutput();
        }

        private GitHubContext GetGitHubContext()
        {
            return new GitHubContext(_Repository, _Credential);
        }

        /// <summary>
        /// Get the character encoding for the specified output encoding.
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static Encoding GetEncoding(OutputEncoding? encoding)
        {
            switch (encoding)
            {
                case OutputEncoding.UTF8:
                    return Encoding.UTF8;

                case OutputEncoding.UTF7:
                    return Encoding.UTF7;

                case OutputEncoding.Unicode:
                    return Encoding.Unicode;

                case OutputEncoding.UTF32:
                    return Encoding.UTF32;

                case OutputEncoding.ASCII:
                    return Encoding.ASCII;

                default:
                    return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            }
        }
    }

    internal sealed class ExportPipeline : PipelineBase
    {
        private const string PROPERTY_BRANCHES = "Branches";
        private const string PROPERTY_LABELS = "Labels";
        private const string PROPERTY_MILESTONES = "Milestones";
        private const string PROPERTY_RELEASES = "Releases";
        private const string PROPERTY_TAGS = "Tags";

        private readonly GitHubContext _ServiceContext;
        private readonly GitHubClient _Client;

        // Track whether Dispose has been called.
        private bool _Disposed = false;

        internal ExportPipeline(PipelineContext context, PipelineWriter writer, GitHubContext serviceContext)
            : base(context, writer)
        {
            _ServiceContext = serviceContext;
            _Client = new GitHubClient(_ServiceContext);
        }

        public override void End()
        {
            for (var i = 0; _ServiceContext.Repository != null && i < _ServiceContext.Repository.Length; i++)
                ProcessRepository(_ServiceContext.Repository[i]);

            base.End();
        }

        internal void ProcessRepository(string repositorySlug)
        {
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
                Writer.WriteObject(repo, false);

                // Write branches as separate objects
                for (var b = 0; b < branches.Length; b++)
                {
                    var branch = PSObject.AsPSObject(branches[b]);
                    Writer.WriteObject(branch, false);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _ServiceContext.Dispose();
                }
                _Disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
