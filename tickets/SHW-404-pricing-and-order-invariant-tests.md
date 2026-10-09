---
id: SHW-404
title: Add edge-case and invariant tests for pricing and orders
type: test
priority: medium
size: S
status: todo
labels: [testing, quality]
depends_on: [SHW-101, SHW-102]
conflicts_with: [SHW-101, SHW-102]
areas: [tests/Shelfwise.Tests/]
---

## Summary
Coverage is thin. Add tests that would have caught the pricing bugs.

## Acceptance criteria
- [ ] Table-driven pricing tests: boundaries around the bulk threshold, many prices x quantities, midpoint rounding
- [ ] Invariant tests: order total always equals the sum of its line totals; total never negative; line total never exceeds `unitPrice * quantity`
- [ ] Conservation test: after any sequence of place/cancel operations, `stock + units in non-cancelled orders` equals the initial stock
- [ ] If any test exposes a real defect not covered by an existing ticket, file it using `tickets/_TEMPLATE.md` instead of silently fixing it
- [ ] `dotnet build` and `dotnet test` pass
