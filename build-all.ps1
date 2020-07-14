param(
    [string]$version = "0.0.0",
    [string]$runtime = "win-x64",
    [string]$config = "release"
)

dotnet test src/analytics-engine.sln -c $config

dotnet build src/analytics-engine/analytics-engine.csproj -c $config -r $runtime -o ./build/$runtime /p:Version=$version 
