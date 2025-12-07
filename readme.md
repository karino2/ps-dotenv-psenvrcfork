- PS-Dotenv: https://github.com/insomnimus/ps-dotenv

# PS-Dotenvrc, Psenvrc fork of dotenv

This is the fork of `PS-Dotenv`, which support `.psenvrc` instead of `.env`. `.env` is not supported.

`.psenvrc` is powershell script.
Store diff of ENV: and unload when go out of directory.

Command Name is almost the same as PS-Dotenv except that Dotenv becomes Dotenvrc.

No bash needs, proper unload after go out.

## Done and Not-Done

DONE

- Load `.psenvrc` which exists in whitelist
- Unload when go out of directory
- Store hash of approved .psenvrc 

## How to use it

In $PROFILE,

```PowerShell
Import-Module Dotenvrc
Invoke-Expression (Get-DotenvrcHook)
```

And `Approve-Dotenvrc .psenvrc` for each file you want to approve. 

## .psenvrc example

.psenvrc is PowerShell script, so you can call subcommand if you want.

```PowerShell
$Env:Path += ';C:\Qt\6.8.3\msvc2022_64\bin\'
$env:MY_QT_LIB_PATH = (qmake -query QT_INSTALL_LIBS).Trim()
$env:MY_QT_INCLUDE_PATH = (qmake -query QT_INSTALL_HEADERS).Trim()
```

## Differences between alternatives

- [direnv – unclutter your .profile - direnv](https://direnv.net/) Original direnv, PowerShell support exists but needs bash.
- [insomnimus/ps-dotenv: A feature complete and unintrusive direnv for Powershell Core](https://github.com/insomnimus/ps-dotenv) Original ps-dotenv, which I forked from. Proper unloading and parent dir search, which portable .env. But no .envrc related feature.
- [takekazuomi/posh-direnv: powershell directory environment switcher](https://github.com/takekazuomi/posh-direnv) Simple .psenvrc loader, no unload and no parent dir search, which make it difficult to cooperate with ZLocation.