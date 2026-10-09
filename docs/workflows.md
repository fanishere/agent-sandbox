# Multi-agent workflow experiments

Each experiment below can be run on this repo. Tag a baseline first so you can reset:

```bash
git tag baseline
# ... run an experiment ...
git reset --hard baseline && git clean -fd     # start over
```

For parallel work, give every agent its own worktree so they cannot trample each other:

```bash
git worktree add .worktrees/SHW-101 -b agent/SHW-101-bulk-discount main
git worktree remove .worktrees/SHW-101          # when merged
```

## 0. Baseline: one agent, one ticket, no help
Run a single agent on SHW-101, then SHW-104, then SHW-204. Record how it does without any
orchestration. Everything else is compared to this.

## 1. Plan -> implement -> review
Three roles: a **planner** reads the ticket and the code and writes a plan (files, steps, risks,
test list); an **implementer** follows the plan; a **reviewer** that has *not* seen the
implementer's reasoning reviews only the diff and the ticket.
Good tickets: SHW-204 (L), SHW-203, SHW-207. Try SHW-303 where the plan itself needs review.

## 2. Test-first split
A **tester** writes failing tests from the ticket alone; an **implementer** makes them pass
without editing the tests. Good tickets: SHW-104, SHW-105, SHW-107, and especially
SHW-106 (the hard part is a deterministic repro).

## 3. Parallel fan-out
An orchestrator reads `python3 scripts/tickets.py batches --max 4`, spawns one agent per
ticket in the first wave, each in its own worktree, then merges. Questions to answer:
how many merge conflicts did the conflict hints miss? Does merge order matter? Who resolves
conflicts, and do they do it correctly?
Variants: (a) respect `conflicts_with`, (b) ignore it, (c) run SHW-302 first, then fan out.

## 4. Adversarial reviewer and ticket triage
Run SHW-602 with a reviewer agent that never sees the ticket list for the seeded bugs
(or `.maintainer/`). Score it against the answer key. Then have a **triager** agent turn its
findings into tickets, deduplicate against the backlog, and set priority.

## 5. Self-service backlog
An orchestrator loop: pick the highest-priority ready ticket, assign it, wait for
`in_review`, dispatch a reviewer, merge on approval, repeat. Stop after N tickets or
when a human is needed. Watch for runaway scope and agents creating tickets for
non-problems.

## 6. Specialists
Domain agent (Domain/ only), API agent (Api/ only), test agent (tests/ only), coordinating
through the ticket and a shared interface contract. Try on SHW-202 and SHW-206, which cross
all three layers.

## What to measure
| Metric | How |
|---|---|
| Correct after first attempt | acceptance criteria all true on first review |
| Review defects found | issues the reviewer raised that were real |
| Escaped defects | seeded bugs / regressions still present afterwards (answer key) |
| Merge conflicts | count, and how they were resolved |
| Scope creep | files touched outside `areas`; unrelated edits |
| Cost and time | tokens, wall-clock, number of agent turns |
| Human interventions | each time you had to step in, and why |

Keep a short log per run in `docs/experiments/<date>-<name>.md` (workflow, tickets, results,
what surprised you).

## Good first sequence
1. Baseline on SHW-101, SHW-104.
2. Test-first split on SHW-106.
3. Plan/implement/review on SHW-204 (after 101 and 102 land).
4. Fan-out wave 1 from `batches`, then compare with running the same tickets serially.
5. SHW-602 reviewer vs the answer key.
