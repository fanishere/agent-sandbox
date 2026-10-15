---
id: SHW-601
title: "Spike: design multi-currency pricing"
type: spike
priority: low
size: S
status: todo
labels: [design, pricing, no-code]
depends_on: []
conflicts_with: []
areas: [docs/design/]
---

## Summary
Product wants to sell in USD, EUR and GBP. Decide how before anyone writes code.

## Deliverable
`docs/design/multi-currency.md` (no production code changes). It must cover:
- Where currency lives in the model (book price, order, line) and how it is represented (`decimal` + code? a `Money` type?)
- Exchange rates: source, caching, rate used at order time, how it is stored on the order
- Rounding rules per currency (JPY has no minor unit) and how they interact with the bulk discount
- API changes (request/response) and backwards compatibility
- Migration plan from the current single-currency data
- Open questions for product, and a recommended breakdown into tickets (use `tickets/_TEMPLATE.md`)

## Acceptance criteria
- [ ] Document exists, is concrete (includes example JSON and a worked rounding example), and states a recommendation, not just options
- [ ] Draft follow-up tickets are added under `tickets/` with status `todo`
