# Shelfwise

A small bookstore inventory-and-orders API in C# / ASP.NET Core. It exists as a
**sandbox for experimenting with multi-agent coding workflows**: a realistic codebase with
rough edges, a backlog of Jira-style tickets, and enough dependencies and file overlap
between tickets to make coordination interesting.

It is not a product. Some of the code is wrong on purpose.

## Quick start

Requires the .NET SDK. The target framework is set in one place, `Directory.Build.props`
(default `net10.0`; change it, and the `Microsoft.AspNetCore.Mvc.Testing` version in
`tests/Shelfwise.Tests/Shelfwise.Tests.csproj`, if you are on 8 or 9).

```bash
dotnet build
dotnet test
dotnet run --project src/Shelfwise.Api    # http://localhost:5080, see src/Shelfwise.Api/Shelfwise.http
```

`dotnet run` uses the Development launch profile, which loads 8 demo books. The tests run
the API in a `Testing` environment with an empty catalog.

All tests should pass on a fresh checkout. Passing tests do **not** mean the code is correct.

## Layout

```
src/Shelfwise.Domain          models, repository interfaces, services (pricing, inventory, orders)
src/Shelfwise.Infrastructure  in-memory repositories, seed data
src/Shelfwise.Api             ASP.NET Core minimal API (Program.cs)
tests/Shelfwise.Tests         xUnit unit tests + in-process API tests
tickets/                      the backlog: one markdown file per ticket, YAML frontmatter
scripts/tickets.py            board tooling (stdlib Python)
docs/workflows.md             experiment ideas and how to run them
AGENTS.md                     the working agreement an agent should follow in this repo
```

## The backlog

27 tickets in `tickets/`: bugs (SHW-1xx), features (2xx), refactors (3xx), chores/tests (4xx),
security (5xx) and spikes (6xx). Each has a type, priority, size, `depends_on`,
`conflicts_with` (likely file overlap) and acceptance criteria. Bug tickets describe
symptoms only, as real bug reports do.

```bash
python3 scripts/tickets.py ready        # unblocked tickets, by priority
python3 scripts/tickets.py batches      # conflict-aware parallel waves
python3 scripts/tickets.py set SHW-101 in_progress
python3 scripts/tickets.py check        # validate frontmatter and dependencies
python3 scripts/tickets.py index        # regenerate tickets/INDEX.md
```

See `docs/workflows.md` for ways to use it.

## Maintainer note

`.maintainer/ANSWER_KEY.md` lists the seeded defects. Keep it out of the agents' hands
if you want an honest test of whether they find things on their own.
