# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#
# PSRule.Rules.Azure module
#

Set-StrictMode -Version latest;

[PSRule.Rules.GitHub.Configuration.PSRuleOption]::UseExecutionContext($ExecutionContext);

#
# Localization
#

#
# Public functions
#

#region Public functions

function Export-GitHubRuleData {
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo])]
    [OutputType([PSObject])]
    param (
        [Parameter(Position = 0, Mandatory = $False)]
        [String]$OutputPath = $PWD,

        # Filter by Repository
        [Parameter(Mandatory = $False)]
        [Alias('R')]
        [String[]]$Repository,

        [Parameter(Mandatory = $False)]
        [PSCredential]$Credential,

        [Parameter(Mandatory = $False)]
        [Switch]$UseGitHubToken
    )
    begin {
        Write-Verbose -Message "[Export-GitHubRuleData] BEGIN::";

        $option = [PSRule.Rules.GitHub.Configuration.PSRuleOption]::new();
        $option.Output.Path = $OutputPath;

        # Build the pipeline
        $builder = [PSRule.Rules.GitHub.Pipeline.PipelineBuilder]::Export($option);
        $builder.Repository($Repository);
        $builder.UseGitHubToken($UseGitHubToken);
        $builder.Credential($Credential);
        $builder.UseCommandRuntime($PSCmdlet);
        $builder.UseExecutionContext($ExecutionContext);
        $pipeline = $builder.Build();
        if ($Null -ne (Get-Variable -Name pipeline -ErrorAction SilentlyContinue)) {
            try {
                $pipeline.Begin();
            }
            catch {
                $pipeline.Dispose();
                throw;
            }
        }
    }
    end {
        if ($Null -ne (Get-Variable -Name pipeline -ErrorAction SilentlyContinue)) {
            try {
                $pipeline.End();
            }
            finally {
                $pipeline.Dispose();
            }
        }
        Write-Verbose -Message "[Export-GitHubRuleData] END::";
    }
}

#endregion Public functions

#
# Export module
#

Export-ModuleMember -Function @(
    'Export-GitHubRuleData'
);
