param(
    [string]$Runtime = "win-x64",
    [string]$Configuration = "Release"
)

Write-Host "Publishing self-contained exe for $Runtime ($Configuration)..."

dotnet restore

dotnet publish `
  -c $Configuration `
  -r $Runtime `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=false `
  -o "publish/$Runtime"

Write-Host "Done. Files: publish/$Runtime"
