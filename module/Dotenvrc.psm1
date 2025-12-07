New-Variable -Option ReadOnly Dotenv ([Dotenv.Daemon]::new())

$ExecutionContext.SessionState.Module.OnRemove += {
	if($global:Dotenv.Enabled) {
		Write-Host "dotenvrc: unloading..."
		$global:Dotenv.Disable()
		remove-item -force -ea silentlyContinue variable:/Dotenv
	}
}

[string]$lastdir = ""

function Update-Dotenvrc {
	[CmdletBinding()]
	param (
		[Parameter(HelpMessage = "Forces the module to reload every psenvrc file if any.")]
		[switch]$Force
	)
	if($pwd.provider.name -ne "FileSystem" -or (!$force -and $pwd.providerpath -eq $script:lastdir)) {
		return
	}
	$script:lastdir = $pwd.providerpath
	if(!$script:Dotenv.Enabled) {
		Write-Host "dotenvrc: not enabled. Call Enable-Dotenvrc first."
		return
	}
	if($force) {
		$script:dotenv.clear()
	}
	$script:dotenv.update($pwd.providerpath)
}

function Enable-Dotenvrc {
	$script:Dotenv.Enabled = $true
}

function Disable-Dotenvrc {
	$script:Dotenv.Enabled = $false
}

function Get-DotenvrcHook {
	@"
if(Test-Path function:/Enable-Dotenvrc) {
	Dotenvrc\Enable-Dotenvrc;
}

function prompt {
	# We check if the command exists to not cause any errors.
	if(Test-Path function:/Update-Dotenvrc) { Dotenvrc\Update-Dotenvrc }

	`$current = Get-Location
	# return normal prompt, maybe we should elaborate here.
	return "PS `$current> "
}
"@
}


function Approve-Dotenvrc {
	[CmdletBinding()]
	param(
		[Parameter(
			Mandatory,
			Position = 0,
			HelpMessage = "Path to an psenvrc file or a directory to whitelist."
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


function Deny-Dotenvrc {
	[CmdletBinding()]
	param(
		[Parameter(
			Mandatory,
			Position = 0,
			HelpMessage = "Path to an psenvrc file to deny."
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


function Debug-Dotenvrc{
	Write-Host "psnenvrc fork"
}

$exports = @{
	Function = @(
		"Update-Dotenvrc"
		"Enable-Dotenvrc"
		"Disable-Dotenvrc"
		"Approve-Dotenvrc"
		"Deny-Dotenvrc"
		"Debug-Dotenvrc"
		"Get-DotenvrcHook"
	)
	Variable = "Dotenv"
	Cmdlet = "Read-Dotenvrc"
}

Export-ModuleMember @exports
