---
id: SHW-207
title: Make POST /orders idempotent with an Idempotency-Key header
type: feature
priority: medium
size: M
status: todo
labels: [orders, api, reliability]
depends_on: [SHW-105]
conflicts_with: [SHW-105, SHW-203, SHW-204, SHW-302]
areas: [src/Shelfwise.Api/, src/Shelfwise.Domain/, tests/Shelfwise.Tests/]
---

## Summary
Mobile clients on flaky networks retry `POST /orders`, creating duplicate orders and
double-reserving stock.

## Details
- Optional `Idempotency-Key` header (1-100 chars)
- Same key + same request body -> return the original response (same status and body), no new order
- Same key + *different* body -> 422
- Keys expire after 24h (use an injectable clock; see SHW-301 if landed)
- Concurrent requests with the same key must result in exactly one order

## Acceptance criteria
- [ ] All rules above, with tests including the concurrent case
- [ ] Failed requests (4xx) are not cached; retrying after fixing the cause works
- [ ] `dotnet build` and `dotnet test` pass
