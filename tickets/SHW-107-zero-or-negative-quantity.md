---
id: SHW-107
title: Orders with a zero or negative quantity cause 500s (and odd stock numbers)
type: bug
priority: high
size: S
status: todo
labels: [orders, validation, inventory]
depends_on: []
conflicts_with: [SHW-105, SHW-201]
areas: [src/Shelfwise.Domain/Services/OrderService.cs, src/Shelfwise.Domain/Services/InventoryService.cs, tests/Shelfwise.Tests/]
---

## Summary
A QA engineer posted `quantity: -5` to `POST /orders` and got an HTTP 500. Afterwards a
colleague noticed the book's stock was higher than before the request.

## Details
Check every quantity of 0, negative, and very large values. Bad input from a client must
produce a clear 400 and must never change inventory.

## Acceptance criteria
- [ ] Quantity <= 0 returns 400 problem-details naming the offending line
- [ ] No inventory change on any rejected request
- [ ] Tests at both the service level and the HTTP level
- [ ] `dotnet build` and `dotnet test` pass

## Out of scope
General request validation for book creation (SHW-201).
