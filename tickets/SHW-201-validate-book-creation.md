---
id: SHW-201
title: Validate input when creating a book
type: feature
priority: high
size: M
status: todo
labels: [catalog, validation, api]
depends_on: []
conflicts_with: [SHW-107, SHW-302]
areas: [src/Shelfwise.Domain/, src/Shelfwise.Api/Program.cs, tests/Shelfwise.Tests/]
---

## Summary
`POST /books` currently accepts anything: blank titles, negative prices, junk ISBNs,
and duplicate ISBNs. The catalog is filling up with bad data.

## Rules
- Title and author: required, trimmed, max 200 chars
- ISBN: valid ISBN-13 (13 digits, correct check digit; hyphens/spaces accepted and normalised away)
- Price: greater than 0, at most 2 decimal places
- Stock quantity: 0 or more
- ISBN must be unique across the catalog (case/format-insensitive)

## Acceptance criteria
- [ ] Invalid input returns 400 `application/problem+json` listing *all* field errors, not just the first
- [ ] Duplicate ISBN returns 409
- [ ] ISBN validation is a unit-tested pure function in the Domain project
- [ ] Existing tests keep passing; note the test helpers currently generate placeholder ISBNs whose check digits are almost always wrong, so they will need fixing
- [ ] `dotnet build` and `dotnet test` pass
