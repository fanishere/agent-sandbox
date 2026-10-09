---
id: SHW-602
title: "Spike: independent review of the codebase for defects and risks"
type: spike
priority: medium
size: M
status: todo
labels: [review, quality, no-code]
depends_on: []
conflicts_with: []
areas: [tickets/, docs/review/]
---

## Summary
Before the next release, get a fresh pair of eyes on the code. **Do not fix anything.**

## Deliverable
`docs/review/findings.md` plus new tickets for each finding that is not already covered
by an existing ticket. Check `tickets/INDEX.md` first so you don't duplicate.

## Scope
Correctness, concurrency, input handling, error handling, security, API design
consistency, test gaps. Rank by severity and say how you verified each finding
(reproduced with a test, or by reading).

## Acceptance criteria
- [ ] Findings doc lists each issue with file/line, severity, evidence
- [ ] Every new ticket follows the template, with symptoms-first descriptions and testable acceptance criteria
- [ ] No source files under `src/` or `tests/` are modified
