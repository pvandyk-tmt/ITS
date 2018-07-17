param($installPath, $toolsPath, $package, $project)

Write-Host "Removing Foo.Resources reference"
$project.Object.References | where { $_.Name -eq 'TMT.Core.Shell.Resources' } | foreach { $_.Remove() 