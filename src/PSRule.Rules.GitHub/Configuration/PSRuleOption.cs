// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Security;

namespace PSRule.Rules.GitHub.Configuration
{
    /// <summary>
    /// A delgate to allow callback to PowerShell to get current working path.
    /// </summary>
    internal delegate string PathDelegate();

    public sealed class PSRuleOption
    {
        internal static readonly PSRuleOption Default = new PSRuleOption
        {
            Output = OutputOption.Default
        };

        /// <summary>
        /// A callback that is overridden by PowerShell so that the current working path can be retrieved.
        /// </summary>
        private static PathDelegate _GetWorkingPath = () => Directory.GetCurrentDirectory();

        public PSRuleOption()
        {
            // Set defaults
            Configuration = new ConfigurationOption();
            Output = new OutputOption();
        }

        public ConfigurationOption Configuration { get; set; }

        /// <summary>
        /// Options that affect how output is generated.
        /// </summary>
        public OutputOption Output { get; set; }

        /// <summary>
        /// Set working path from PowerShell host environment.
        /// </summary>
        /// <param name="executionContext">An $ExecutionContext object.</param>
        /// <remarks>
        /// Called from PowerShell.
        /// </remarks>
        public static void UseExecutionContext(EngineIntrinsics executionContext)
        {
            if (executionContext == null)
            {
                _GetWorkingPath = () => Directory.GetCurrentDirectory();
                return;
            }
            _GetWorkingPath = () => executionContext.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        public static string GetWorkingPath()
        {
            return _GetWorkingPath();
        }

        /// <summary>
        /// Get a full path instead of a relative path that may be passed from PowerShell.
        /// </summary>
        internal static string GetRootedPath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(GetWorkingPath(), path));
        }

        /// <summary>
        /// Get a full path instead of a relative path that may be passed from PowerShell.
        /// </summary>
        internal static string GetRootedBasePath(string path)
        {
            var rootedPath = GetRootedPath(path);
            if (rootedPath.Length > 0 && IsSeparator(rootedPath[rootedPath.Length - 1]))
                return rootedPath;

            return string.Concat(rootedPath, Path.DirectorySeparatorChar);
        }

        internal static bool TryGetEnvironmentVariableString(string variable, out string value)
        {
            value = null;
            var v = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(v))
                return false;

            value = v;
            return true;
        }

        internal static bool TryGetEnvironmentVariableSecureString(string variable, out SecureString value)
        {
            value = null;
            var v = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(v))
                return false;

            value = new NetworkCredential("na", v).SecurePassword;
            return true;
        }

        [DebuggerStepThrough]
        private static bool IsSeparator(char c)
        {
            return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
        }
    }
}
