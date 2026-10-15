#!/usr/bin/env python3
"""Tiny ticket board tooling for tickets/*.md (stdlib only).

Usage:
  scripts/tickets.py list [--status todo] [--type bug]
  scripts/tickets.py show SHW-101
  scripts/tickets.py ready                 tickets that are todo and whose deps are all done
  scripts/tickets.py batches [--max N]     suggested waves of tickets that can run in parallel
                                           without conflicts_with overlaps (greedy, by priority)
  scripts/tickets.py set SHW-101 done      update the status field in place
  scripts/tickets.py check                 validate ids, dependencies, statuses
  scripts/tickets.py index                 regenerate tickets/INDEX.md
"""
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
TICKET_DIR = ROOT / "tickets"
STATUSES = ["todo", "in_progress", "in_review", "blocked", "done"]
PRIORITY_ORDER = {"critical": 0, "high": 1, "medium": 2, "low": 3}
SIZE_ORDER = {"XS": 0, "S": 1, "M": 2, "L": 3, "XL": 4}


class Ticket:
    def __init__(self, path: Path, meta: dict[str, object]):
        self.path = path
        self.meta = meta

    def __getitem__(self, key: str):
        return self.meta.get(key, [] if key in ("labels", "depends_on", "conflicts_with", "areas") else "")

    @property
    def id(self) -> str:
        return str(self["id"])

    @property
    def sort_key(self):
        return (PRIORITY_ORDER.get(str(self["priority"]), 9), SIZE_ORDER.get(str(self["size"]), 9), self.id)


def _parse_value(raw: str):
    raw = re.sub(r"\s+#.*$", "", raw).strip()
    if raw.startswith("[") and raw.endswith("]"):
        inner = raw[1:-1].strip()
        return [p.strip() for p in inner.split(",") if p.strip()] if inner else []
    if len(raw) >= 2 and raw[0] == raw[-1] and raw[0] in "\"'":
        return raw[1:-1]
    return raw


def load_tickets() -> list[Ticket]:
    tickets = []
    for path in sorted(TICKET_DIR.glob("SHW-*.md")):
        text = path.read_text(encoding="utf-8")
        match = re.match(r"^---\n(.*?)\n---\n", text, re.S)
        if not match:
            raise SystemExit(f"{path.name}: missing frontmatter")
        meta: dict[str, object] = {}
        for line in match.group(1).splitlines():
            if not line.strip() or line.lstrip().startswith("#"):
                continue
            key, _, value = line.partition(":")
            meta[key.strip()] = _parse_value(value)
        tickets.append(Ticket(path, meta))
    return tickets


def by_id(tickets: list[Ticket]) -> dict[str, Ticket]:
    return {t.id: t for t in tickets}


def ready(tickets: list[Ticket]) -> list[Ticket]:
    index = by_id(tickets)
    done = {t.id for t in tickets if t["status"] == "done"}
    result = [
        t
        for t in tickets
        if t["status"] == "todo" and all(dep in done for dep in t["depends_on"] if dep in index)
    ]
    return sorted(result, key=lambda t: t.sort_key)


def fmt_row(t: Ticket) -> str:
    return f"{t.id:8} {str(t['type']):9} {str(t['priority']):9} {str(t['size']):3} {str(t['status']):12} {t['title']}"


def cmd_list(args, tickets):
    for t in sorted(tickets, key=lambda t: t.id):
        if args.status and t["status"] != args.status:
            continue
        if args.type and t["type"] != args.type:
            continue
        print(fmt_row(t))


def cmd_show(args, tickets):
    t = by_id(tickets).get(args.id)
    if not t:
        raise SystemExit(f"unknown ticket {args.id}")
    print(t.path.read_text(encoding="utf-8"))


def cmd_ready(args, tickets):
    for t in ready(tickets):
        print(fmt_row(t))


def cmd_batches(args, tickets):
    index = by_id(tickets)
    done = {t.id for t in tickets if t["status"] == "done"}
    remaining = [t for t in tickets if t["status"] == "todo"]
    wave_no = 0
    while remaining:
        candidates = sorted(
            [t for t in remaining if all(d in done for d in t["depends_on"] if d in index)],
            key=lambda t: t.sort_key,
        )
        if not candidates:
            print("\nUnschedulable (cyclic or blocked dependencies):", ", ".join(t.id for t in remaining))
            return
        wave: list[Ticket] = []
        for t in candidates:
            clash = any(o.id in t["conflicts_with"] or t.id in o["conflicts_with"] for o in wave)
            if not clash and (not args.max or len(wave) < args.max):
                wave.append(t)
        wave_no += 1
        print(f"Wave {wave_no}  ({len(wave)} tickets in parallel)")
        for t in wave:
            print("  " + fmt_row(t))
        deferred = [t.id for t in candidates if t not in wave]
        if deferred:
            print("  deferred (conflicts/limit): " + ", ".join(deferred))
        print()
        done.update(t.id for t in wave)
        remaining = [t for t in remaining if t not in wave]


def cmd_set(args, tickets):
    if args.status not in STATUSES:
        raise SystemExit(f"status must be one of {STATUSES}")
    t = by_id(tickets).get(args.id)
    if not t:
        raise SystemExit(f"unknown ticket {args.id}")
    text = t.path.read_text(encoding="utf-8")
    new, n = re.subn(r"(?m)^status:\s*\S+", f"status: {args.status}", text, count=1)
    if n != 1:
        raise SystemExit("could not find status line")
    t.path.write_text(new, encoding="utf-8")
    print(f"{args.id} -> {args.status}")


def cmd_check(args, tickets):
    problems = []
    warnings = []
    index = by_id(tickets)
    seen = set()
    for t in tickets:
        if t.id in seen:
            problems.append(f"duplicate id {t.id}")
        seen.add(t.id)
        if not t.path.name.startswith(t.id + "-"):
            problems.append(f"{t.path.name}: filename does not start with {t.id}-")
        if t["status"] not in STATUSES:
            problems.append(f"{t.id}: bad status {t['status']!r}")
        if t["priority"] not in PRIORITY_ORDER:
            problems.append(f"{t.id}: bad priority {t['priority']!r}")
        if t["size"] not in SIZE_ORDER:
            problems.append(f"{t.id}: bad size {t['size']!r}")
        for field in ("depends_on", "conflicts_with"):
            for ref in t[field]:
                if ref not in index:
                    if (TICKET_DIR / "parked").glob(f"{ref}-*.md"):
                        warnings.append(f"{t.id}: {field} references parked {ref}")
                    else:
                        problems.append(f"{t.id}: {field} references unknown {ref}")
        if t.id in t["depends_on"]:
            problems.append(f"{t.id}: depends on itself")
    # cycle detection
    state: dict[str, int] = {}

    def visit(i: str, stack: list[str]):
        if state.get(i) == 2:
            return
        if state.get(i) == 1:
            problems.append("dependency cycle: " + " -> ".join(stack + [i]))
            return
        state[i] = 1
        for d in index[i]["depends_on"]:
            if d in index:
                visit(d, stack + [i])
        state[i] = 2

    for i in index:
        visit(i, [])
    if problems:
        print("\n".join(problems))
        raise SystemExit(1)
    for w in warnings:
        print(f"warning: {w}")
    print(f"ok: {len(tickets)} tickets")


def cmd_index(args, tickets):
    lines = ["# Ticket board", "", "_Generated by `scripts/tickets.py index`. Do not edit by hand._", ""]
    for status in STATUSES:
        group = sorted([t for t in tickets if t["status"] == status], key=lambda t: t.id)
        lines.append(f"## {status.replace('_', ' ').title()} ({len(group)})")
        lines.append("")
        if not group:
            lines += ["_none_", ""]
            continue
        lines += ["| ID | Type | Pri | Size | Title | Depends on |", "|---|---|---|---|---|---|"]
        for t in group:
            deps = ", ".join(t["depends_on"]) or ""
            lines.append(
                f"| [{t.id}]({t.path.name}) | {t['type']} | {t['priority']} | {t['size']} | {t['title']} | {deps} |"
            )
        lines.append("")
    lines += ["## Dependency graph", "", "```mermaid", "graph LR"]
    for t in sorted(tickets, key=lambda t: t.id):
        if t["depends_on"]:
            for d in t["depends_on"]:
                lines.append(f"  {d.replace('-', '_')} --> {t.id.replace('-', '_')}")
    lines += ["```", ""]
    (TICKET_DIR / "INDEX.md").write_text("\n".join(lines), encoding="utf-8")
    print("wrote tickets/INDEX.md")


def main():
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = p.add_subparsers(dest="cmd", required=True)
    s = sub.add_parser("list")
    s.add_argument("--status")
    s.add_argument("--type")
    s.set_defaults(fn=cmd_list)
    s = sub.add_parser("show")
    s.add_argument("id")
    s.set_defaults(fn=cmd_show)
    sub.add_parser("ready").set_defaults(fn=cmd_ready)
    s = sub.add_parser("batches")
    s.add_argument("--max", type=int, default=0, help="max tickets per wave")
    s.set_defaults(fn=cmd_batches)
    s = sub.add_parser("set")
    s.add_argument("id")
    s.add_argument("status")
    s.set_defaults(fn=cmd_set)
    sub.add_parser("check").set_defaults(fn=cmd_check)
    sub.add_parser("index").set_defaults(fn=cmd_index)
    args = p.parse_args()
    args.fn(args, load_tickets())


if __name__ == "__main__":
    sys.exit(main())
