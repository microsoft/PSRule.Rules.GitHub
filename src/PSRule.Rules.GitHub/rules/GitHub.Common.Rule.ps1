# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

function global:GetRepositoryBranch {
    [CmdletBinding()]
    [OutputType([PSObject[]])]
    param (
        [Parameter(Mandatory = $False)]
        [String]$Name
    )
    process {
        $branches = @($TargetObject.Branches | Where-Object {
            [String]::IsNullOrEmpty($Name) -or $Name -eq $_.Name
        });
        return $branches;
    }
}
