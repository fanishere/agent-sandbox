---
id: SHW-301
title: Replace direct DateTime.Now/UtcNow calls with an injectable TimeProvider
type: refactor
priority: medium
size: S
status: todo
labels: [tech-debt, testability]
depends_on: []
conflicts_with: [SHW-104, SHW-105, SHW-302]
areas: [src/Shelfwise.Domain/Services/, src/Shelfwise.Api/Program.cs, tests/Shelfwise.Tests/]
---

## Summary
Services read the system clock directly, so time-dependent behaviour can't be tested
and timestamps are inconsistent: some are UTC, at least one is server-local time,
so `addedAt` values from different code paths don't compare correctly.

## Acceptance criteria
- [ ] No production code calls `DateTime.Now`, `DateTime.UtcNow` or `DateTimeOffset.UtcNow` directly (a test or analyzer rule enforcing this is a bonus)
- [ ] Services take `System.TimeProvider` through their constructors; the API registers `TimeProvider.System`
- [ ] All stored timestamps are UTC
- [ ] Tests use a controllable time provider and at least one test asserts a timestamp
- [ ] `dotnet build` and `dotnet test` pass

## Notes
Constructor changes will ripple through `TestApp.cs`; expect merge conflicts with other tickets.
