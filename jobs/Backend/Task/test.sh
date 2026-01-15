#!/usr/bin/env bash
set -e

# Use .NET 10 SDK if available in home directory
if [[ -d "$HOME/.dotnet" ]]; then
    export PATH="$HOME/.dotnet:$PATH"
fi

echo "Running tests..."
dotnet test
