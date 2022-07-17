---
external help file: PSRule.Rules.GitHub-help.xml
Module Name: PSRule.Rules.GitHub
online version: https://github.com/microsoft/PSRule.Rules.GitHub/blob/main/docs/commands/PSRule.Rules.GitHub/en-US/Export-GitHubRuleData.md
schema: 2.0.0
---

# Export-GitHubRuleData

## SYNOPSIS

Export GitHub repository configuration.

## SYNTAX

```text
Export-GitHubRuleData [[-OutputPath] <String>] [-Repository <String[]>] [-Credential <PSCredential>]
 [-UseGitHubToken] [<CommonParameters>]
```

## DESCRIPTION

Export configuration data from one or more GitHub repositories.

## EXAMPLES

### Example 1

```powershell
Export-GitHubRuleData -Repository 'microsoft/PSRule';
```

Exports repository configuration data for `microsoft/PSRule`.

## PARAMETERS

### -Credential

An optional credential to use against GitHub APIs.
This parameter uses the credential password as a Personal Access Token (PAT), username is ignored.
If not specified, unauthenticated access will be used.
The data exported will be limited to the permissions of the credential used.

Please note that the GitHub APIs are throttled aggressively for non-authenticated requests.

```yaml
Type: PSCredential
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputPath

The path to store generated JSON files containing configuration data.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Repository

The repository spec for one or more repositories to export configuration data for.
Use `<organization>/<repository_name>` or `<user>/<repository_name>` to export a single repository.
Multiple repositories can be exported by comma separating each.
To export all repositories for a user or organization use `<organization>/` or `<user>/`.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: R

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UseGitHubToken

Determines if the GITHUB_TOKEN environment variable is used as a credential.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.IO.FileInfo

### System.Management.Automation.PSObject

## NOTES

## RELATED LINKS
