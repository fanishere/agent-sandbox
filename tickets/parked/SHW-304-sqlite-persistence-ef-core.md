---
id: SHW-304
title: Persist data to SQLite with EF Core
type: feature
priority: medium
size: XL
status: todo
labels: [epic, infrastructure, persistence]
depends_on: [SHW-106]
conflicts_with: [SHW-202, SHW-206, SHW-204, SHW-207]
areas: [src/Shelfwise.Infrastructure/, src/Shelfwise.Api/, tests/Shelfwise.Tests/]
---

## Summary
Everything is lost on restart. Replace the in-memory repositories with EF Core + SQLite.

## Details
This is an epic: it should be broken into sub-tickets (suggested: DbContext + mappings,
migrations, repository implementations, concurrency control for stock, test fixtures,
configuration/connection strings, seeding) before implementation starts.

## Acceptance criteria
- [ ] Data survives an application restart
- [ ] Schema managed by EF migrations checked into the repo
- [ ] Stock updates are safe under concurrency at the database level (optimistic concurrency token or transactional update)
- [ ] The in-memory implementation remains available for fast unit tests
- [ ] Integration tests run against a real SQLite database (temp file or in-memory connection), isolated per test class
- [ ] No behavioural changes visible through the HTTP API
- [ ] `dotnet build` and `dotnet test` pass

## Notes
Needs NuGet access for `Microsoft.EntityFrameworkCore.Sqlite` and `.Design`.
