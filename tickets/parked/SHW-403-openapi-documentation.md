---
id: SHW-403
title: Publish an OpenAPI document with accurate response metadata
type: chore
priority: low
size: S
status: todo
labels: [api, docs]
depends_on: []
conflicts_with: [SHW-302, SHW-202, SHW-402]
areas: [src/Shelfwise.Api/]
---

## Summary
Front-end developers are reading the source to learn the API.

## Acceptance criteria
- [ ] OpenAPI document served at `/openapi/v1.json` (Development only)
- [ ] Every endpoint has a name, tag, summary and declared response types for each status code it can actually return (200/201/400/404/409)
- [ ] Problem-details responses are documented
- [ ] A test fetches the document and asserts every route in the app appears in it
- [ ] `dotnet build` and `dotnet test` pass
