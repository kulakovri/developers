#!/usr/bin/env bash
set -e

echo "Running ExchangeRateUpdater..."
dotnet run --project ExchangeRateUpdater.csproj
