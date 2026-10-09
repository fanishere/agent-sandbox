---
id: SHW-205
title: Low-stock report endpoint
type: feature
priority: low
size: S
status: todo
labels: [reporting, inventory, api]
depends_on: []
conflicts_with: [SHW-302]
areas: [src/Shelfwise.Domain/Services/, src/Shelfwise.Api/Program.cs, src/Shelfwise.Api/appsettings.json, tests/Shelfwise.Tests/]
---

## Summary
Purchasing wants a list of books that need reordering.

## Details
`GET /reports/low-stock?threshold=N` returns books with `stockQuantity <= N`, lowest stock
first, ties broken by title. If `threshold` is omitted use the configured default
(`Inventory:LowStockThreshold`, default 5). Negative threshold -> 400.

## Acceptance criteria
- [ ] Default comes from configuration via the options pattern (`IOptions<T>`), not a constant
- [ ] Response includes id, title, isbn, stockQuantity
- [ ] Tests for default, explicit threshold, ordering, empty result, bad input
- [ ] `dotnet build` and `dotnet test` pass
