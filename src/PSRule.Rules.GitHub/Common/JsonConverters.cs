// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using PSRule.Rules.GitHub.Pipeline;
using PSRule.Rules.GitHub.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSRule.Rules.GitHub
{
    /// <summary>
    /// A custom serializer to correctly convert PSObject properties to JSON instead of CLIXML.
    /// </summary>
    internal sealed class PSObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PSObject);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is PSObject obj))
                throw new ArgumentException(message: PSRuleResources.SerializeNullPSObject, paramName: nameof(value));

            if (value is FileSystemInfo fileSystemInfo)
            {
                WriteFileSystemInfo(writer, fileSystemInfo, serializer);
                return;
            }
            writer.WriteStartObject();
            foreach (var property in obj.Properties)
            {
                // Ignore properties that are not readable or can cause race condition
                if (!property.IsGettable || property.Value is PSDriveInfo || property.Value is ProviderInfo || property.Value is DirectoryInfo)
                    continue;

                writer.WritePropertyName(property.Name);
                serializer.Serialize(writer, property.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Create target object based on JObject
            var result = existingValue as PSObject ?? new PSObject();

            // Read tokens
            ReadObject(value: result, reader: reader);
            return result;
        }

        private static void ReadObject(PSObject value, JsonReader reader)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new PipelineSerializationException(PSRuleResources.ReadJsonFailed);

            reader.Read();
            string name = null;

            // Read each token
            while (reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        name = reader.Value.ToString();
                        break;

                    case JsonToken.StartObject:
                        var child = new PSObject();
                        ReadObject(value: child, reader: reader);
                        value.Properties.Add(new PSNoteProperty(name: name, value: child));
                        break;

                    case JsonToken.StartArray:
                        var items = new List<object>();
                        reader.Read();

                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            items.Add(ReadValue(reader));
                            reader.Read();
                        }

                        value.Properties.Add(new PSNoteProperty(name: name, value: items.ToArray()));
                        break;

                    default:
                        value.Properties.Add(new PSNoteProperty(name: name, value: reader.Value));
                        break;
                }
                reader.Read();
            }
        }

        private static object ReadValue(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return reader.Value;

            var value = new PSObject();
            ReadObject(value, reader);
            return value;
        }

        private static void WriteFileSystemInfo(JsonWriter writer, FileSystemInfo value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.FullName);
        }
    }

    internal sealed class GitHubCommunityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Data.CommunityProfile);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Create target object based on JObject
            var result = existingValue as Data.CommunityProfile ?? new Data.CommunityProfile();

            // Read tokens
            if (reader.TokenType != JsonToken.StartObject)
                throw new PipelineSerializationException(PSRuleResources.ReadJsonFailed);

            reader.Read();
            string name = null;

            // Read each token
            while (reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        name = reader.Value.ToString();
                        break;

                    case JsonToken.StartObject:
                        if (name == "files")
                        {
                            ReadFiles(result, reader);
                        }
                        break;
                }
                reader.Read();
            }
            return result;
        }

        private static void ReadFiles(Data.CommunityProfile value, JsonReader reader)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new PipelineSerializationException(PSRuleResources.ReadJsonFailed);

            reader.Read();
            string name = null;

            // Read each token
            while (reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        name = reader.Value.ToString();
                        break;

                    case JsonToken.Null:
                        if (name == "issue_template")
                            value.IssueTemplate = true;

                        break;

                    case JsonToken.StartObject:
                        switch (name)
                        {
                            case "code_of_conduct":
                                value.CodeOfConduct = true;
                                break;

                            case "contributing":
                                value.Contributing = true;
                                break;

                            case "issue_template":
                                value.IssueTemplate = true;
                                break;

                            case "pull_request_template":
                                value.PullRequestTemplate = true;
                                break;

                            case "license":
                                value.License = true;
                                break;

                            case "readme":
                                value.ReadMe = true;
                                break;

                            default:
                                break;
                        }
                        reader.Skip();
                        break;
                }
                reader.Read();
            }
        }
    }
}
