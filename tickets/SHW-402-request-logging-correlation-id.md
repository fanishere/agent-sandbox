---
id: SHW-402
title: Structured request logging with correlation IDs
type: feature
priority: low
size: S
status: todo
labels: [observability, api]
depends_on: []
conflicts_with: [SHW-302, SHW-403]
areas: [src/Shelfwise.Api/, tests/Shelfwise.Tests/]
---

## Summary
We can't trace a customer's problem through the logs.

## Acceptance criteria
- [ ] Middleware accepts an incoming `X-Correlation-Id` header or generates one, and echoes it on the response
- [ ] The id is attached to the logging scope so every log line in the request carries it
- [ ] One structured log line per request: method, path, status code, duration in ms
- [ ] Domain exceptions mapped to 4xx are logged at Information/Warning; unexpected exceptions at Error
- [ ] Incoming ids are length-limited and sanitised (no log-injection via newlines)
- [ ] Tests assert the header behaviour, including the sanitising rule
- [ ] `dotnet build` and `dotnet test` pass
