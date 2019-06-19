Write-Host "##vso[task.setvariable variable=ApplicationBinariesDirectory]$PSScriptRoot"
$meta = Get-Content -Path "$PSScriptRoot/app-package.json" | ConvertFrom-Json
$meta | Get-Member -MemberType NoteProperty | ForEach-Object {
    $key = $_.Name
    $value = $meta."$key"
    Write-Host "##vso[task.setvariable variable=$key]$value"
}