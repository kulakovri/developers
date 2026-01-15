#!/usr/bin/env bash
set -e

# Use .NET 10 SDK if available in home directory
if [[ -d "$HOME/.dotnet" ]]; then
    export PATH="$HOME/.dotnet:$PATH"
fi

if [[ "$1" == "-v" || "$1" == "--verbose" ]]; then
    env Logging__LogLevel__ExchangeRateUpdater=Debug \
        dotnet run --project ExchangeRateUpdater.csproj
else
    dotnet run --project ExchangeRateUpdater.csproj
fi
