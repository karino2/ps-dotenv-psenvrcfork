---
external help file: Dotenvrc-help.xml
Module Name: Dotenvrc
online version:
schema: 2.0.0
---

# Deny-Dotenvrc

## SYNOPSIS
Unauthorizes an env file.

## SYNTAX

```
Deny-Dotenvrc [-Path] <String[]> [<CommonParameters>]
```

## DESCRIPTION
Removes a file from the list of authorized files.
This only has an effect with the safe mode enabled.
With the safe mode, files not explicitly allowed by you will not be sourced.

## EXAMPLES

### Example 1
```powershell
PS C:\> Deny-Dotenvrc ~\.psenvrc
```

This example unauthorizes `~\.psenvrc`.

## PARAMETERS

### -Path
Path to an psenvrc file to deny.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
