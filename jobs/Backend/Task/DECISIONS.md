# Decision Notes

## Framework

- Chose **.NET 10** as a current, supported runtime.
- Kept the implementation simple — no AOT, trimming, or advanced resilience patterns.

## Data Source

- **Czech National Bank (CNB) daily fixing** — official, public, no API key, plain-text format.
- Alternatives (ECB, Fixer.io) add complexity (XML, auth, rate limits).

## Exchange Rate Rules

- Only source-defined rates are returned (no inverse/cross rates computed).
- Rationale: calculated rates introduce rounding errors; assignment requires source-only.

## Error Handling

- Network errors and timeouts throw `InvalidOperationException` with URL for diagnostics.
- Fail-fast is appropriate for a console app; retries/circuit breakers omitted.

## Sync vs Async

- `GetExchangeRates` is synchronous (sync-over-async via `GetAwaiter().GetResult()`).
- Acceptable for a single-threaded console app with no request pool.

## Not Implemented

- Caching (stateless is simpler for a one-shot app)
- Retry policies / circuit breakers
- Integration tests against live CNB endpoint
- Historical rates
