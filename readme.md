- PS-Dotenv: https://github.com/insomnimus/ps-dotenv

# PS-Dotenv-Psenvrc fork

This is the fork of `PS-Dotenv`, which support `.psenvrc` instead of `.env`. `.env` is not supported.

`.psenvrc` is powershell script.
Store diff of ENV: and unload when go out of directory.

Command Name is almost the same as PS-Dotenv though it's not `.env` anymore.

No bash needs, proper unload after go out.

## Done and Not-Done

DONE

- Load `.psenvrc` which exists in whitelist
- Unload when go out of directory

Not-DONE

- store whitelist

## How to use it

In $PROFILE,

```PowerShell
Import-Module Dotenvrc
Invoke-Expression (Get-DotenvHook)
```

And `Approve-Dotenvrc .psenvrc` for each file you want to approve. 

## .psenvrc example

.psenvrc is PowerShell script, so you can call subcommand if you want.

```PowerShell
$Env:Path += ';C:\Qt\6.8.3\msvc2022_64\bin\'
$env:MY_QT_LIB_PATH = (qmake -query QT_INSTALL_LIBS).Trim()
$env:MY_QT_INCLUDE_PATH = (qmake -query QT_INSTALL_HEADERS).Trim()
```