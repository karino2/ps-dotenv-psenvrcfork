---
external help file: Dotenvrc-help.xml
Module Name: Dotenvrc
online version:
schema: 2.0.0
---

# Approve-Dotenvrc

## SYNOPSIS
Whitelists a particular env file for dotenv.

## SYNTAX

```
Approve-Dotenvrc [-Path] <String[]> [<CommonParameters>]
```

## DESCRIPTION
Whitelists a particular env file for dotenv.
This only has an effect with the safe mode enabled.
With the safe mode, files not explicitly allowed by you will not be sourced.

## EXAMPLES

### Example 1
```powershell
PS C:\> Approve-Dotenvrc ~\.psenvrc
```

This example authorizes the file located at `~\.psenvrc`.

## PARAMETERS

### -Path
Path to an env file to allow.

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
