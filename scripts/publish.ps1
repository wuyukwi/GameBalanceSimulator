param(
    [Parameter()]
    [ValidateSet("win-x64", "win-arm64", "osx-x64", "osx-arm64", "linux-x64", "linux-arm64")]
    [string]$Runtime = "win-x64",

    [Parameter()]
    [string]$Output = "publish"
)

$ErrorActionPreference = "Stop"
$Project = "src/GameBalanceSimulator/GameBalanceSimulator.csproj"

dotnet publish $Project `
    -c Release `
    -r $Runtime `
    --self-contained true `
    -o $Output `
    /p:PublishSingleFile=true `
    /p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$executable = if ($Runtime.StartsWith("win")) { "GameBalanceSimulator.exe" } else { "GameBalanceSimulator" }
Write-Host ""
Write-Host "Published successfully to: $Output/$executable"
