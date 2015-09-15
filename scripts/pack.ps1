$root = $env:APPVEYOR_BUILD_FOLDER
$versionStr = $env:appveyor_build_version

$content = (Get-Content $root\Toppler.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\Toppler.compiled.nuspec

& nuget pack $root\Toppler.compiled.nuspec