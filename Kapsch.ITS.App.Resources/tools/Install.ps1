param($installPath, $toolsPath, $package, $project)
$resourcesDll = Join-Path $installPath "lib\TMT.Core.Shell.Resources.dll"
Write-Host "Adding resources DLL from " $resourcesDll

if ($project.Kind -eq "{E24C65DC-7377-472b-9ABA-BC803B73C61A}") {
    $project.Object.References.AddFromFile($resourcesDll)
} else {    
    $project.Object.References.Add($resourcesDll)
}