// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Management.Automation;
using System.Security;
using System.Text;
using PSRule.Rules.GitHub.Configuration;
using PSRule.Rules.GitHub.Pipeline.Output;

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
                if (PSRuleOption.TryGetEnvironmentVariableString(GITHUB_REPOSITORY, out var repo))
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
                if (PSRuleOption.TryGetEnvironmentVariableSecureString(GITHUB_TOKEN, out var token))
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
        private readonly GitHubContext _ServiceContext;
        private readonly RepositoryHelper _Helper;

        // Track whether Dispose has been called.
        private bool _Disposed;

        internal ExportPipeline(PipelineContext context, PipelineWriter writer, GitHubContext serviceContext)
            : base(context, writer)
        {
            _ServiceContext = serviceContext;
            _Helper = new RepositoryHelper(serviceContext);
        }

        public override void End()
        {
            for (var i = 0; _ServiceContext.Repository != null && i < _ServiceContext.Repository.Length; i++)
                ProcessRepository(_ServiceContext.Repository[i]);

            base.End();
        }

        internal void ProcessRepository(string repositorySlug)
        {
            var o = _Helper.Get(repositorySlug);
            if (o.Length > 0)
                Writer.WriteObject(o, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                    _ServiceContext.Dispose();

                _Disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
