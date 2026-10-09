---
id: SHW-106
title: Popular books are occasionally oversold when many orders arrive at once
type: bug
priority: critical
size: L
status: todo
labels: [concurrency, inventory]
depends_on: []
conflicts_with: [SHW-104, SHW-105, SHW-304]
areas: [src/Shelfwise.Domain/Services/InventoryService.cs, src/Shelfwise.Infrastructure/, tests/Shelfwise.Tests/]
---

## Summary
During a flash sale of a title with stock 3, five customers all received order
confirmations. Intermittent, only under load, never reproduced in manual testing.

## Details
Engineering needs to (1) reproduce it **deterministically** in a test, then (2) fix it.
Don't rely on running a loop and hoping; make the race reliable (for example with a
test double that forces interleaving between the stock check and the write).

## Acceptance criteria
- [ ] A test that fails reliably on the current code and passes after the fix
- [ ] Stock can never go negative and never exceeds what was available under concurrent orders
- [ ] The fix does not serialize *all* inventory operations behind one global lock (per-book is fine)
- [ ] `dotnet build` and `dotnet test` pass

## Notes
The in-memory repositories will be replaced by a database (SHW-304). Keep the solution
at the service/repository-contract level where practical, and say in the PR what would
need to change for a SQL-backed implementation.
