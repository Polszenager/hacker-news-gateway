# Future improvements — responsiveness, load control, ops & infosec

Quick actionable items (short-term)
1. Require pagination / async job for very large requests.
2. Add observability (metrics/logging) and per-client rate limiting.

Operational & InfoSec considerations (non-code)
- Dependency hygiene
  - Vet and pin third-party libs (e.g., `Hacker-News-Net`) and add automated dependency scanning (Dependabot, GitHub Actions).
- Observability and alerts
  - Export metrics (requests/sec to HN, latency, cache hit rate, circuit-breaker state).
  - Alert on rising error rates or open circuit-breakers.
- Governance
  - Document acceptable usage (rate limits, contact) in your API docs. Provide a background job or bulk-report endpoint for very large requests.

Mid-term / further improvements
- Enforce per-client quotas and require API keys for high-volume usage so abuse can be controlled.
- Use Redis for a global cache and request coalescing (singleflight) across instances.
- Introduce a background job queue for bulk requests and notify clients when a job completes.
- Add advanced observability (OpenTelemetry) and downstream tracing.

Notes on adopting `Hacker-News-Net` third party
- Pros: reduces client maintenance effort, provides models and edge-case handling.
- Cons: external dependency to vet and maintain; wrap it behind an adapter to keep your `IHackerNewsService` contract stable.

