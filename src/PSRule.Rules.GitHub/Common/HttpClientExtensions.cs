// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Net.Http;

namespace PSRule.Rules.GitHub
{
    internal static class HttpClientExtensions
    {
        public static T Get<T>(this HttpClient client, string requestUri, string[] headers)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
            for (var i = 0; i < headers.Length; i++)
                message.Headers.Accept.ParseAdd(headers[i]);

            var requestTask = client.SendAsync(message);
            requestTask.Wait();
            var response = requestTask.Result;
            response.EnsureSuccessStatusCode();
            var contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new GitHubCommunityConverter());
            return JsonConvert.DeserializeObject<T>(contentTask.Result, settings);
        }
    }
}
