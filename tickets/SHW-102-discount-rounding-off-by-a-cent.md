---
id: SHW-102
title: Discounted line totals are sometimes one cent too low
type: bug
priority: medium
size: XS
status: done
labels: [pricing, finance]
depends_on: []
conflicts_with: [SHW-101, SHW-204]
areas: [src/Shelfwise.Domain/Services/PricingService.cs, tests/Shelfwise.Tests/PricingServiceTests.cs]
---

## Summary
Finance reconciliation found that some bulk-discounted lines are 1 cent lower than the
invoice our accountants compute by hand (standard "round half up" at 2 decimals).

## Details
Example: 11 copies of a book priced 5.15. Gross 56.65, minus 10% = 50.985.
Finance expects **50.99**; the API returns **50.98**.

## Acceptance criteria
- [ ] 11 x 5.15 prices to 50.99
- [ ] Midpoints (x.xx5) always round away from zero, for several different cases
- [ ] Behaviour is covered by tests (Theory with multiple data rows)
- [ ] `dotnet build` and `dotnet test` pass

## Out of scope
Currency handling (see SHW-601).
