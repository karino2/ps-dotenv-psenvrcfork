New-Variable -Option ReadOnly Dotenv ([Dotenv.Daemon]::new())

$ExecutionContext.SessionState.Module.OnRemove += {
	if($global:Dotenv.Enabled) {
		Write-Host "dotenv: unloading..."
		$global:Dotenv.Disable()
		remove-item -force -ea silentlyContinue variable:/Dotenv
	}
}

[string]$lastdir = ""

function Clear-DotenvJobs {
	Get-Job -State Completed `
	| Where-Object { $_.name -eq "Dotenv" } `
	| Remove-Job
}

function Update-Dotenv {
	[CmdletBinding()]
	param (
		[Parameter(HelpMessage = "Forces the module to reload every env file if any.")]
		[switch]$Force
	)
	if($pwd.provider.name -ne "FileSystem" -or (!$force -and $pwd.providerpath -eq $script:lastdir)) {
		return
	}
	$script:lastdir = $pwd.providerpath
	if(!$script:Dotenv.Enabled) {
		Write-Host "dotenv: not enabled. Call Enable-Dotenv first."
		return
	}
	if($script:Dotenv.Async) {
		Clear-DotenvJobs
		$null = Start-ThreadJob -Name "Dotenv" -ArgumentList $script:Dotenv, $force {
			param(
				[Dotenv.Daemon]$Daemon,
				[bool]$force
			)
			if($force) {
				$daemon.Clear()
			}
			$Daemon.Update($pwd.providerpath)
		}
	} else {
		if($force) {
			$script:dotenv.clear()
		}
		$script:dotenv.update($pwd.providerpath)
	}
}

function Enable-Dotenv {
	$script:Dotenv.Enabled = $true
}

function Disable-Dotenv {
	$script:Dotenv.Enabled = $false
}

function Disable-DotenvAsync {
	$script:Dotenv.Async = $false
}

function Get-DotenvHook {
	@"
if(Test-Path function:/Enable-Dotenv) {
	Dotenvrc\Enable-Dotenv;
	Dotenvrc\Disable-DotenvAsync;
}

function prompt {
	# We check if the command exists to not cause any errors.
	if(Test-Path function:/Update-Dotenv) { Dotenvrc\Update-Dotenv }

	`$current = Get-Location
	# 通常のプロンプトを返す
	return "PS `$current> "
}
"@
}


function Approve-DotenvFile {
	[CmdletBinding()]
	param(
		[Parameter(
			Mandatory,
			Position = 0,
			HelpMessage = "Path to an env file or a directory to whitelist."
		)]
		[string[]]$Path
	)
	$yes = $false
	foreach($f in $path) {
		$f = [System.IO.Path]::GetFullPath($f, $pwd.providerpath)
		if($script:Dotenv.AuthorizePattern($f, $false)) {
			write-information "allowed $f"
			$yes = $true
		} else {
			write-warning "$f is already allowed"
		}
	}
	if($yes) {
		script:update-dotenv -force
	}
}


function Deny-DotenvFile {
	[CmdletBinding()]
	param(
		[Parameter(
			Mandatory,
			Position = 0,
			HelpMessage = "Path to an env file to deny."
		)]
		[ArgumentCompleter({ $script:Dotenv.WhitePaths | where-object { [WildcardPattern]::ContainsWildcardCharacters("$_") } | sort-object })]
		[string[]]$Path
	)
	$yes = $false
	foreach($f in $path) {
		$f = [System.IO.Path]::GetFullPath($f, $pwd.providerpath)
		if($script:Dotenv.UnauthorizePattern($f, $false)) {
			write-information "denied $f"
			$yes = $true
		} else {
			write-warning "$f is not allowed"
		}
	}
	if($yes) {
		script:update-dotenv -force
	}
}


function Debug-Dotenv{
	Write-Host "psnenvrc fork"
}

$exports = @{
	Function = @(
		"Update-Dotenv"
		"Enable-Dotenv"
		"Disable-Dotenv"
		"Approve-DotenvFile"
		"Deny-DotenvFile"
		"Debug-Dotenv"
		"Disable-DotenvAsync"
		"Get-DotenvHook"
	)
	Variable = "Dotenv"
	Cmdlet = "Read-Dotenv"
}

Export-ModuleMember @exports
