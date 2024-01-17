// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using PSRule.Rules.GitHub.Configuration;

namespace PSRule.Rules.GitHub.Pipeline;

internal sealed class PipelineContext
{
    internal readonly PSRuleOption Option;

    public PipelineContext(PSRuleOption option)
    {
        Option = option;
    }
}
