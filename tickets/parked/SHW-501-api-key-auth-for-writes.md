---
id: SHW-501
title: Require an API key for mutating catalog and fulfilment endpoints
type: security
priority: high
size: M
status: todo
labels: [security, api]
depends_on: []
conflicts_with: [SHW-302, SHW-502]
areas: [src/Shelfwise.Api/, tests/Shelfwise.Tests/]
---

## Summary
Anyone on the internet can add books, mark orders paid and ship them.

## Details
Protect: `POST /books`, `POST /orders/{id}/pay`, `/ship`, `/cancel`.
Leave open: all GETs, `GET /health`, and `POST /orders` (customers place orders).
Keys come from configuration (`Auth:ApiKeys`, a list), sent in the `X-Api-Key` header.

## Acceptance criteria
- [ ] Missing or wrong key -> 401 problem-details; no information about valid keys leaks
- [ ] Key comparison is constant-time
- [ ] Keys are never logged
- [ ] Implemented with the ASP.NET Core authentication/authorization system (scheme + policy), not an ad-hoc `if` in each endpoint
- [ ] Test project supplies a test key via configuration; tests cover allowed and denied cases for every protected endpoint
- [ ] `dotnet build` and `dotnet test` pass
