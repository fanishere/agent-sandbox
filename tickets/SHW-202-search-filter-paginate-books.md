---
id: SHW-202
title: Search, filter and paginate the book list
type: feature
priority: medium
size: M
status: todo
labels: [catalog, api]
depends_on: []
conflicts_with: [SHW-103, SHW-302, SHW-403]
areas: [src/Shelfwise.Domain/, src/Shelfwise.Infrastructure/, src/Shelfwise.Api/Program.cs, tests/Shelfwise.Tests/]
---

## Summary
`GET /books` returns the entire catalog. The storefront needs search and paging.

## Details
Query parameters (all optional):
- `q`: case-insensitive substring match on title or author
- `genre`: case-insensitive exact match
- `inStock`: `true` hides books with zero stock
- `sort`: `title` (default), `price`, `-price`, `added`
- `page` (default 1) and `pageSize` (default 20, max 100)

Response becomes an envelope:
`{ "items": [...], "page": 1, "pageSize": 20, "totalCount": 123 }`

## Acceptance criteria
- [ ] All parameters work alone and combined
- [ ] Invalid `page`/`pageSize`/`sort` return 400, not 500
- [ ] Filtering/sorting lives behind the repository or service interface (not in the endpoint lambda)
- [ ] Tests cover each parameter, combinations and boundaries
- [ ] Existing tests updated for the new response shape
- [ ] `dotnet build` and `dotnet test` pass
