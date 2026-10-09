---
id: SHW-204
title: Support discount codes on orders
type: feature
priority: medium
size: L
status: todo
labels: [pricing, orders, api]
depends_on: [SHW-101, SHW-102]
conflicts_with: [SHW-203, SHW-302, SHW-207]
areas: [src/Shelfwise.Domain/, src/Shelfwise.Infrastructure/, src/Shelfwise.Api/Program.cs, tests/Shelfwise.Tests/]
---

## Summary
Marketing wants to run promotions with codes like `SPRING10`.

## Details
- A code is either a percentage off the order subtotal or a fixed amount off
- Optional expiry date and optional maximum number of redemptions
- Case-insensitive on lookup
- Applies to the order **after** per-line bulk discounts; the total can never go below 0
- Admin endpoint `POST /discount-codes` to create; `POST /orders` accepts optional `discountCode`
- The order response shows the code applied and the amount discounted
- Invalid, expired or exhausted code -> 400 with a specific reason (do not silently ignore it)
- Cancelling an order gives its redemption back

## Acceptance criteria
- [ ] All rules above implemented with unit + HTTP tests
- [ ] Redemption counting is safe under concurrent orders (see SHW-106 for how not to do it)
- [ ] Rounding follows the same rule as line pricing
- [ ] `dotnet build` and `dotnet test` pass

## Notes
Large enough that a planner agent should split it before anyone starts coding.
