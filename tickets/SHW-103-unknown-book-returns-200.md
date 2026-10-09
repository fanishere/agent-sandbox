---
id: SHW-103
title: GET /books/{id} returns 200 for books that don't exist
type: bug
priority: medium
size: XS
status: todo
labels: [api, catalog]
depends_on: []
conflicts_with: [SHW-302, SHW-202]
areas: [src/Shelfwise.Api/Program.cs, tests/Shelfwise.Tests/ApiTests.cs]
---

## Summary
The mobile app can't tell "book deleted" from "book exists" because looking up an
unknown id comes back as a success.

## Details
`GET /books/<random guid>` -> HTTP 200 with a body of `null`. `GET /orders/<random guid>`
correctly returns 404 with a problem-details body, and the book endpoint should behave
the same way.

## Acceptance criteria
- [ ] Unknown book id returns 404 `application/problem+json` consistent with the order endpoint
- [ ] Existing book id still returns 200 with the book
- [ ] Integration test covers both
- [ ] `dotnet build` and `dotnet test` pass
