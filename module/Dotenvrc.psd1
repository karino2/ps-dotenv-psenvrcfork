@{
	RootModule = "Dotenvrc.psm1"
	ModuleVersion = "1.0.0"
	Author = "Kazuma Arino<hogeika2@gmail.com>"
	CompatiblePSEditions = @("Core")
	GUID = '16dd6f1b-7e4e-45fb-80d4-fbc5b18ad0a5'
	PowerShellVersion = "6.0"

	FunctionsToExport = @(
		"Update-Dotenv"
		"Register-DotenvName"
		"Unregister-DotenvName"
		"Enable-Dotenv"
		"Disable-Dotenv"
		"Approve-DotenvFile"
		"Deny-DotenvFile"
		"Debug-Dotenv"
		"Disable-DotenvAsync"
		"Get-DotenvHook"
	)
	CmdletsToExport = @("Read-Dotenv")
	VariablesToExport = @("Dotenv")
	AliasesToExport = @()

	NestedModules = @("Dotenvrc.dll")
}
