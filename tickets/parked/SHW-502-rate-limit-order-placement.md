---
id: SHW-502
title: Rate limit POST /orders per client
type: security
priority: low
size: S
status: todo
labels: [security, api, reliability]
depends_on: []
conflicts_with: [SHW-302, SHW-501]
areas: [src/Shelfwise.Api/, tests/Shelfwise.Tests/]
---

## Summary
A script hammering `POST /orders` can drain stock for every popular title.

## Acceptance criteria
- [ ] Built-in ASP.NET Core rate limiting middleware, fixed or sliding window, 10 requests/minute per client IP by default
- [ ] Limits configurable via `appsettings.json`
- [ ] Exceeded limit returns 429 with a `Retry-After` header and a problem-details body
- [ ] Other endpoints are not limited
- [ ] Tests configure a tiny limit and assert the 429 behaviour
- [ ] `dotnet build` and `dotnet test` pass
