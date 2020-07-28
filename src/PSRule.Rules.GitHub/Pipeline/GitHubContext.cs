// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Octokit;
using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PSRule.Rules.GitHub.Pipeline
{
    internal sealed class GitHubContext : IDisposable
    {
        // Details for user-agent header
        private const string GITHUB_PRODUCT_HEADER = "PSRule.Rules.GitHub";
        private static readonly string ProductVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        private readonly PSCredential _Credential;

        private GitHubHandler _GitHubHandler;

        private bool _Disposed;

        public GitHubContext(string[] repository, PSCredential credential)
        {
            Repository = repository;
            _Credential = credential;
            _GitHubHandler = new GitHubHandler();
        }

        public string[] Repository { get; set; }

        public Octokit.GitHubClient GetClient()
        {
            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(GITHUB_PRODUCT_HEADER, ProductVersion));
            if (_Credential != null)
                client.Credentials = new Credentials(_Credential.GetNetworkCredential().Password);

            return client;
        }

        public HttpClient GetHttpClient()
        {
            var client = new HttpClient(_GitHubHandler);
            if (_Credential != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _Credential.GetNetworkCredential().Password);

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(GITHUB_PRODUCT_HEADER, ProductVersion));
            return client;
        }

        private sealed class GitHubHandler : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                AddHeaders(request);
                return base.SendAsync(request, cancellationToken);
            }

            private static void AddHeaders(HttpRequestMessage request)
            {
                
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _GitHubHandler.Dispose();
                }
                _Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
