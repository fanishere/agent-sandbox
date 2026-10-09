---
id: SHW-302
title: Split Program.cs into endpoint modules and a service-registration extension
type: refactor
priority: low
size: M
status: todo
labels: [tech-debt, api, high-conflict]
depends_on: []
conflicts_with: [SHW-103, SHW-201, SHW-202, SHW-203, SHW-204, SHW-205, SHW-206, SHW-207, SHW-402, SHW-403, SHW-501, SHW-502]
areas: [src/Shelfwise.Api/]
---

## Summary
`Program.cs` holds DI wiring, error handling, seeding and every endpoint. Every API change
touches it. Split it so features land in separate files.

## Target structure
- `Endpoints/CatalogEndpoints.cs`, `Endpoints/OrderEndpoints.cs`, `Endpoints/HealthEndpoints.cs`, each exposing a `Map...()` extension on `IEndpointRouteBuilder`
- `Infrastructure/ServiceCollectionExtensions.cs` (or similar) for DI registration
- `Middleware/ProblemDetailsMiddleware.cs` for exception-to-HTTP translation
- `Program.cs` reads like a table of contents (< 40 lines)

## Acceptance criteria
- [ ] Behaviour is unchanged; all existing tests pass without modification
- [ ] No route, status code or payload changes
- [ ] `dotnet build` and `dotnet test` pass

## Notes
Deliberately the highest merge-conflict-risk ticket. A good experiment: run it alone
first, then fan out the API tickets; or run it concurrently and watch what happens.
