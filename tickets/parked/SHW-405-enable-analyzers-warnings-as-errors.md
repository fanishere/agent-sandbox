---
id: SHW-405
title: Turn on .NET analyzers and treat warnings as errors
type: chore
priority: low
size: S
status: todo
labels: [quality, tech-debt, high-conflict]
depends_on: []
conflicts_with: [SHW-301, SHW-302, SHW-303]
areas: [Directory.Build.props, .editorconfig, src/, tests/]
---

## Summary
Raise the baseline: enable the analyzers, fix what they find, and keep it that way.

## Acceptance criteria
- [ ] `AnalysisLevel` set to latest recommended; `TreatWarningsAsErrors` true in `Directory.Build.props`
- [ ] All new warnings fixed properly; suppressions only with a comment explaining why
- [ ] `dotnet build` is warning-free
- [ ] `.editorconfig` severity settings documented in a short note in AGENTS.md
- [ ] `dotnet test` passes
