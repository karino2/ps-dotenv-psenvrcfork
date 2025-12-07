# Dotenvrc

## Installation and Setup
See the [readme](readme.md).

## The `$Dotenvrc` variable.
This variable is exported and holds the configuration for the entire module.
You cannot remove or replace it (without force) but you can modify its properties, which take effect immediately.

For example, instead of running `Disable-Dotenvrc`, you can run `$Dotenvrc.Enabled = $false`.

## Configuration
The `$Dotenvrc` variable is how you configure the modules behaviour.
These are its fields:

-	`Enabled`: Turns the module on and off.
-	`LoggingPreference`: Configures how the logs are written, you can tab through its fields.
-	`SkipErrors`: If set to true, errors encountered during parsing env files will cause the parser to skip to the next line instead of returning.
-	`SafeMode`: If enabled, only the files explicitly allowed will be sourced.
-	`Quiet`: If set to `$true`, disables info messages while the safe mode is enabled and there are unauthorized files in the current directory or its parents.

## Read-Dotenvrc
Parses an env file. The parsed variables are not sourced, the caller is expected to do it. You don't have to call this command, the module uses it under the hood.

## Disable-Dotenvrc
Disables the module without removing it from the session. Equivalent to `$Dotenvrc.Enabled = $true`.

## Enable-Dotenvrc
Enables the module back. Equivalent to `$Dotenvrc.Enabled = $true`.

## Update-Dotenvrc
Triggers the module to check for env files in the current and parent directories. This is the entrypoint to this module. This command is meant to be called automatically by your `Prompt` function.

## Approve-Dotenvrc
Whitelists a particular env file for dotenv. This only has an effect with the safe mode enabled. With the safe mode, files not explicitly allowed by you will not be sourced.

## Deny-Dotenvrc
Removes a file from the list of authorized files. This only has an effect with the safe mode enabled. With the safe mode, files not explicitly allowed by you will not be sourced.

## Clear-Dotenvrc
Clear all approved files info and unload all environment variables loaded by .psenvrc.

## Get-DotenvrcHook
Show default hook code for PS-Dotenvrc.
This code is custom prompt function which call Update-Dotenvrc for each prompt.
Use this function with Invoke-Expression in $PROFILE file for setup.
