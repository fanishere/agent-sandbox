---
id: SHW-303
title: Replace exceptions-as-control-flow in the domain with a Result type
type: refactor
priority: low
size: L
status: todo
labels: [tech-debt, design, domain]
depends_on: [SHW-104, SHW-105, SHW-107]
conflicts_with: [SHW-203, SHW-204, SHW-302]
areas: [src/Shelfwise.Domain/, src/Shelfwise.Api/, tests/Shelfwise.Tests/]
---

## Summary
Not-found, validation and out-of-stock conditions are expected outcomes, but they are
modelled as exceptions and translated to HTTP by middleware. That hides failure modes
from method signatures and makes the services awkward to compose.

## Details
**Design first.** Before writing code, produce a short proposal (in the PR description or
`docs/design/result-type.md`) covering: the shape of the result type (and whether to write
one or adopt a library), the error taxonomy, how errors map to HTTP, and the migration
order. Get it reviewed, then implement.

## Acceptance criteria
- [ ] Domain services return results for expected failures; exceptions remain only for bugs
- [ ] HTTP behaviour (status codes, problem-details shape) is unchanged
- [ ] Existing tests are migrated, not deleted
- [ ] `dotnet build` and `dotnet test` pass
