---
external help file: Dotenvrc-help.xml
Module Name: Dotenvrc
online version:
schema: 2.0.0
---

# Get-DotenvrcHook

## SYNOPSIS
Show default hook code for PS-Dotenvrc

## SYNTAX

```
Get-DotenvrcHook
```

## DESCRIPTION
Show default hook code for PS-Dotenvrc.
This code is custom prompt function which call Update-Dotenvrc for each prompt.
Use this function with Invoke-Expression in $PROFILE file for setup.

## EXAMPLES

### Example 1
```powershell
PS C:\> Invoke-Expression (Get-DotenvrcHook)
```

## PARAMETERS

## INPUTS

### None

## OUTPUTS

### System.String
## NOTES

## RELATED LINKS
