---
id: SHW-203
title: Enforce valid order status transitions
type: feature
priority: high
size: M
status: todo
labels: [orders, domain]
depends_on: [SHW-104]
conflicts_with: [SHW-104, SHW-302, SHW-207]
areas: [src/Shelfwise.Domain/Models/Order.cs, src/Shelfwise.Domain/Services/OrderService.cs, tests/Shelfwise.Tests/]
---

## Summary
Right now any endpoint can move an order to any status: a cancelled order can be shipped,
a shipped order can be cancelled. We need an explicit lifecycle.

## Allowed transitions
```
Pending -> Paid -> Shipped
Pending -> Cancelled
Paid    -> Cancelled   (stock restocked; refunds are out of scope)
```
Everything else is rejected. Repeating a transition to the state the order is already in
is also rejected, with one exception: cancelling an already-cancelled order is a no-op
that returns the order unchanged (idempotent, no second restock).

## Acceptance criteria
- [ ] Transition rules live on the domain model (or a dedicated type), not in endpoint code
- [ ] Illegal transition returns 409 with a message naming the current and requested status
- [ ] Exhaustive test of the full from/to matrix
- [ ] `dotnet build` and `dotnet test` pass
