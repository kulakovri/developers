# Exchange Rate Updater

A .NET 10 console application that fetches daily foreign currency exchange rates from the Czech National Bank (CNB). It filters rates by a configurable list of currencies and outputs them to the console in a human-readable format.

## Behavior Rules

- **Source-defined rates only**: Returns only rates published by CNB (e.g., `EUR -> CZK`).
- **No inverse rates**: `CZK -> EUR` is NOT computed as `1 / rate`.
- **No cross rates**: `USD -> EUR` is NOT derived via `USD -> CZK -> EUR`.
- **CZK required**: CZK must be in the requested currency list (it is the target currency for all rates).
- **Missing currencies ignored**: Unknown or unavailable currency codes are silently skipped.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (pinned via `global.json`)

## How to Run

```bash
./run.sh
```

Or manually:

```bash
dotnet run --project ExchangeRateUpdater.csproj
```

## How to Test

```bash
./test.sh
```

Or manually:

```bash
dotnet test
```

## Configuration

The CNB API base URL can be configured via:

| Method | Key / Variable | Example |
|--------|----------------|---------|
| appsettings.json | `CnbApi:BaseUrl` | `"https://www.cnb.cz"` |
| Environment variable | `CnbApi__BaseUrl` | `export CnbApi__BaseUrl=https://www.cnb.cz` |

If neither is set, defaults to `https://www.cnb.cz`.

## Architecture

`Program.cs` wires DI and logging. `ExchangeRateProvider` orchestrates fetch (via `IExchangeRatesSource`) and parse (via `CnbRatesParser`). The `IExchangeRatesSource` abstraction enables unit testing with a fake implementation.

## Design Decisions

See [DECISIONS.md](DECISIONS.md) for detailed rationale on data source choice, error handling, caching trade-offs, and extensibility considerations.
