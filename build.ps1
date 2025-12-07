#!/bin/env pwsh -f
param(
	[switch]$release
)

$ErrorActionPreference = "stop"

function starts-with-space($s) {
	$s.length -eq 0 -or
	[char]::IsWhitespace($s.chars(0))
}

function build-project($cfg = "debug", $out) {
	$output = dotnet publish `
		--nologo `
		--configuration $cfg `
		--output $out 2>&1
	while(starts-with-space $output[0]) {
		$output = $output[1..($output.length)]
	}

	$output | % {
		$_.replace("$PSScriptRoot\", "")
	}
}

$out = "$PSScriptRoot/bin/Dotenvrc"

if(test-path -pathType container $out) {
	remove-item -recurse $out
}

$cfg = if($release) { "release" } else { "debug" }
build-project $cfg $out

if($lastexitcode -eq 0) {
	copy-item -recurse -force "$PSScriptRoot/module/*" $out
	remove-item "$out/build_doc.ps1"
	echo "built the module into $out"
}
