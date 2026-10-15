---
id: SHW-104
title: Cancelling an order doesn't put the books back on the shelf
type: bug
priority: high
size: S
status: in_review
labels: [orders, inventory]
depends_on: []
conflicts_with: [SHW-105, SHW-203, SHW-106]
areas: [src/Shelfwise.Domain/Services/OrderService.cs, tests/Shelfwise.Tests/OrderServiceTests.cs]
---

## Summary
The warehouse team notices stock counts drifting down over time. After a customer
cancels, the copies they ordered never become available again.

## Details
Repro: book with stock 5 -> order 2 (stock 3) -> cancel order -> stock is still 3,
expected 5.

## Acceptance criteria
- [ ] Cancelling an order returns every line's quantity to inventory
- [ ] Cancelling the same order twice does not restock twice (second call must not change stock)
- [ ] Tests cover single-line, multi-line and double-cancel
- [ ] `dotnet build` and `dotnet test` pass

## Out of scope
Which statuses may be cancelled at all (see SHW-203).
