# Working agreement for agents in this repo

## Commands
```bash
dotnet build                      # must succeed with no new warnings
dotnet test                       # must pass before you hand work back
dotnet format --verify-no-changes # style check (if the SDK includes it)
python3 scripts/tickets.py ...    # ticket board (see README)
```

## Architecture rules
- `Shelfwise.Domain` has **no** dependency on the other projects or on ASP.NET Core.
- `Shelfwise.Infrastructure` implements Domain interfaces; it may depend on Domain only.
- `Shelfwise.Api` is a thin HTTP layer: parse input, call a service, shape the response.
  Business rules belong in Domain.
- Money is `decimal`. Never `double`/`float`.
- Timestamps are UTC.
- Public behaviour changes need a test; bug fixes need a test that fails without the fix.

## Code style
- File-scoped namespaces, nullable reference types on, one top-level type per file for new code
  (existing multi-type files may stay as they are unless your ticket says otherwise).
- Prefer small, focused changes. Do not reformat or rename unrelated code.
- No new NuGet packages unless the ticket needs them; say which and why in your summary.

## Working on a ticket
1. Read the ticket in `tickets/` fully, including `depends_on` and `conflicts_with`.
2. Work on a branch named `agent/<ticket-id>-<slug>` (e.g. `agent/SHW-101-bulk-discount`).
3. `python3 scripts/tickets.py set <id> in_progress` when you start.
4. Keep the change scoped to the ticket. If you find another problem, **do not fix it**;
   write it up as a new ticket in `tickets/` using `tickets/_TEMPLATE.md` (next free id in the
   right range) and mention it in your summary.
5. Make every acceptance criterion true and tested. Run `dotnet build` and `dotnet test`.
6. `python3 scripts/tickets.py set <id> in_review`, commit with message `<ticket-id>: <summary>`.
7. Summary for the reviewer: what changed, how you verified it, anything you were unsure about.

Only mark a ticket `done` after review, and never edit other tickets' status or content
(except to add a new ticket file).

## Do not read
`.maintainer/` is for the humans running the experiment.
