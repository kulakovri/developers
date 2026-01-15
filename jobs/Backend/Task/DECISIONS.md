# Decision Notes -- Exchange Rate Provider

## 1. Data Source Choice

- **Czech National Bank (CNB) daily fixing** selected as the authoritative source.
- Reasons:
  - Official, publicly available, no API key required.
  - Simple plain-text format (pipe-delimited), easy to parse.
  - Reliable uptime; data updates once daily at ~14:30 CET.
  - Suitable for a read-only task with no write/mutation requirements.
- Alternative sources (ECB, Fixer.io, Open Exchange Rates) were considered but add complexity (XML parsing, authentication, rate limits).

## 2. Exchange Rate Rules

- **Only source-defined rates are returned** -- if CNB publishes `EUR -> CZK`, we return it as-is.
- **No inverse rates**: `CZK -> EUR` is NOT computed as `1 / rate`.
- **No cross rates**: `USD -> EUR` is NOT derived via `USD -> CZK -> EUR`.
- Rationale:
  - Calculated rates introduce rounding errors and deviate from official values.
  - The assignment explicitly requires returning only what the source provides.
  - Keeps the provider predictable and auditable.

## 3. Error Handling & Reliability

- Network errors (`HttpRequestException`) and timeouts (`TaskCanceledException`) are caught and wrapped into `InvalidOperationException` with the URL for diagnostics.
- Exceptions bubble up to the caller (`Program.Main`) which handles them with a user-friendly message.
- Rationale:
  - For a console app, fail-fast is appropriate -- no silent failures.
  - Retries and circuit breakers would add complexity beyond the scope of a take-home task.

## 4. Concurrency & Performance

- **Expected usage**: single-threaded console app, called once per run.
- **No async public API**: `GetExchangeRates` is synchronous.
  - Sync-over-async (`GetAwaiter().GetResult()`) is acceptable here -- no UI thread, no request pool to block.
- **What would change in production**:
  - Expose `async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync()`.
  - Inject `HttpClient` via DI (or `IHttpClientFactory`) for testability and lifecycle management.
  - Consider parallel fetches if multiple sources are added.

## 5. Caching (Not Implemented)

- **Intentionally omitted** to keep the solution minimal and stateless.
- **Where to add**: wrap or replace `GetLatestRatesContent()` with a caching layer.
- **Possible strategies**:
  - In-memory cache with TTL (e.g., `MemoryCache`, 1-hour expiry).
  - Cache key: URL or date string (`yyyy-MM-dd`) since CNB updates daily.
  - For distributed systems: Redis with sliding expiration.
- **Trade-off**: caching adds state; for a one-shot console app, the overhead isn't justified.

## 6. Extensibility

If this were to evolve into a production service:

| Concern | Approach |
|---------|----------|
| Async API | Add `GetExchangeRatesAsync` alongside sync version |
| Testability | Inject `HttpClient` or `IExchangeRateSource` interface |
| Multiple sources | Strategy pattern; aggregate or fallback between CNB, ECB, etc. |
| Background refresh | Hosted service polling on schedule, populating shared cache |
| Distributed cache | Redis/Memcached with pub/sub invalidation |

---

## Out of Scope

The following were deliberately not implemented:

- Integration tests hitting the real CNB endpoint.
- Retry policies (Polly or similar).
- Logging / structured telemetry.
- Rate limiting or circuit breaker.
- Distributed caching.
- Historical rates (CNB supports date parameter; not required here).
