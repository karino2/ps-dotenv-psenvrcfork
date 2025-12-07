#!/bin/env pwsh -f

Measure-PlatyPSMarkdown -Path ../docs/*.md | Import-MarkdownCommandHelp -Path {$_.FilePath} | Export-MamlCommandHelp -OutputFolder ./maml
mv -Force maml/Dotenvrc/* en-us/
rmdir -r maml
