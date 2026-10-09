---
id: SHW-101
title: Customers buying exactly 10 copies don't get the bulk discount
type: bug
priority: high
size: XS
status: todo
labels: [pricing, domain]
depends_on: []
conflicts_with: [SHW-102, SHW-204]
areas: [src/Shelfwise.Domain/Services/PricingService.cs, tests/Shelfwise.Tests/PricingServiceTests.cs]
---

## Summary
Our storefront says "10% off when you buy 10 or more copies of a title". A customer
emailed support: they ordered exactly 10 copies and were charged full price. 11 copies
get the discount.

## Details
Reported by support (ticket CS-4412). Reproduce with any book priced at 10.00:
`POST /orders` with quantity 10 -> expect line total 90.00, got 100.00.

## Acceptance criteria
- [ ] Quantity 10 receives the 10% discount; quantity 9 does not
- [ ] A test pins the boundary (9, 10, 11)
- [ ] `dotnet build` and `dotnet test` pass

## Out of scope
Changing the discount percentage or threshold, rounding behaviour (see SHW-102).
