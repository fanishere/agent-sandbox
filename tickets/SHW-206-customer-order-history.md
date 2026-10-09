---
id: SHW-206
title: List and filter orders (customer order history)
type: feature
priority: medium
size: M
status: todo
labels: [orders, api]
depends_on: []
conflicts_with: [SHW-302, SHW-304]
areas: [src/Shelfwise.Domain/Repositories/, src/Shelfwise.Infrastructure/, src/Shelfwise.Api/Program.cs, tests/Shelfwise.Tests/]
---

## Summary
Customers need to see their past orders, and support needs to search orders.

## Details
`GET /orders` with optional filters: `customerEmail` (case-insensitive exact),
`status`, `from` and `to` (inclusive, on creation date, ISO-8601). Newest first.
Paged with `page`/`pageSize` using the same envelope as SHW-202 (if that ticket has landed,
reuse its types; otherwise define them in a way SHW-202 can adopt).

## Acceptance criteria
- [ ] Each filter works alone and combined; unknown `status` value -> 400
- [ ] `from` after `to` -> 400
- [ ] Filtering happens in the repository layer, not by loading every order into the endpoint
- [ ] Tests for filters, ordering and paging
- [ ] `dotnet build` and `dotnet test` pass
