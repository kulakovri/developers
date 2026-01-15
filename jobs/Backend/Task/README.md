# Exchange Rate Updater

A .NET 6 console application that fetches daily foreign currency exchange rates from the Czech National Bank (CNB). It filters rates by a configurable list of currencies and outputs them to the console in a human-readable format.

## Behavior Rules

- **Source-defined rates only**: Returns only rates published by CNB (e.g., `EUR -> CZK`).
- **No inverse rates**: `CZK -> EUR` is NOT computed as `1 / rate`.
- **No cross rates**: `USD -> EUR` is NOT derived via `USD -> CZK -> EUR`.
- **CZK required**: CZK must be in the requested currency list (it is the target currency for all rates).
- **Missing currencies ignored**: Unknown or unavailable currency codes are silently skipped.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

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

```
Program.cs
    |
    +-- DI wiring (ServiceCollection)
    |
    v
ExchangeRateProvider (orchestration)
    |
    +-- IExchangeRatesSource (interface)
    |       |
    |       +-- CnbRatesSource (HTTP fetch)
    |
    +-- CnbRatesParser (static, parsing logic)
```

| Component | Responsibility |
|-----------|----------------|
| `CnbRatesSource` | Fetches raw text from CNB daily.txt endpoint |
| `CnbRatesParser` | Parses pipe-delimited text into `ExchangeRate` objects |
| `ExchangeRateProvider` | Orchestrates fetch + parse, filters by requested currencies |
| `Program.cs` | Configures DI, resolves provider, outputs results |

**Tests** are split into:
- `ExchangeRateProviderTests` -- orchestration logic (uses fake source)
- `CnbRatesParserTests` -- parsing logic (pure functions)

## Design Decisions

See [DECISIONS.md](DECISIONS.md) for detailed rationale on data source choice, error handling, caching trade-offs, and extensibility considerations.
