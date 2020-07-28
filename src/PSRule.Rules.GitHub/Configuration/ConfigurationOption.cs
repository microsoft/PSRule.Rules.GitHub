// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using YamlDotNet.Serialization;

namespace PSRule.Rules.GitHub.Configuration
{
    /// <summary>
    /// A set of configuration values that can be used within rule definitions.
    /// </summary>
    public sealed class ConfigurationOption
    {
        public ConfigurationOption()
        {
            DefaultOrg = null;
        }

        public ConfigurationOption(ConfigurationOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            DefaultOrg = option.DefaultOrg;
        }

        public override bool Equals(object obj)
        {
            return obj is ConfigurationOption option && Equals(option);
        }

        public bool Equals(ConfigurationOption other)
        {
            return other != null &&
                DefaultOrg == other.DefaultOrg;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine
            {
                int hash = 17;
                hash = hash * 23 + (DefaultOrg != null ? DefaultOrg.GetHashCode() : 0);
                return hash;
            }
        }

        [DefaultValue(null)]
        [YamlMember(Alias = "GitHub_DefaultOrg")]
        public string DefaultOrg { get; set; }
    }
}
