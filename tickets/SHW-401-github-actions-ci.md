---
id: SHW-401
title: Add a GitHub Actions CI workflow
type: chore
priority: low
size: XS
status: todo
labels: [ci, infrastructure]
depends_on: []
conflicts_with: []
areas: [.github/workflows/]
---

## Summary
There is no CI. Add a workflow that builds and tests on every push and pull request.

## Acceptance criteria
- [ ] `.github/workflows/ci.yml` triggers on push to `main` and on pull requests
- [ ] Uses `actions/setup-dotnet` with the SDK version taken from `Directory.Build.props`' target framework (document how)
- [ ] Steps: restore, build (`--no-restore`, Release), test (`--no-build`), publish test results as an artifact
- [ ] NuGet packages cached
- [ ] `dotnet format --verify-no-changes` runs as a separate, non-blocking job
- [ ] README gets a build-status badge placeholder
