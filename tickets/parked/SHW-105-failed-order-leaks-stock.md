---
id: SHW-105
title: A rejected multi-book order still removes stock for the books it did reserve
type: bug
priority: high
size: S
status: todo
labels: [orders, inventory]
depends_on: []
conflicts_with: [SHW-104, SHW-106, SHW-207]
areas: [src/Shelfwise.Domain/Services/OrderService.cs, tests/Shelfwise.Tests/OrderServiceTests.cs]
---

## Summary
Ops found "ghost" stock loss: books vanish from inventory with no matching order.

## Details
Repro: book A stock 5, book B stock 1. Order A x2 + B x3. The API returns 409
(insufficient stock for B) as expected, but `GET /books` now shows A with stock 3.
No order exists. The same happens when a later line references an unknown book id.

## Acceptance criteria
- [ ] If any line of an order fails, inventory for all lines is unchanged
- [ ] No order is persisted when placement fails
- [ ] Tests cover: insufficient stock on a later line, unknown book on a later line
- [ ] `dotnet build` and `dotnet test` pass
