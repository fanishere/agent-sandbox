# Answer key (humans only)

Defects planted in the base code, and the ticket that describes each.

| Ticket | Where | Defect |
|---|---|---|
| SHW-101 | `PricingService.PriceLine` | `quantity > BulkThreshold` should be `>=` (docs say "10 or more") |
| SHW-102 | `PricingService.PriceLine` | `Math.Round` defaults to banker's rounding; needs `MidpointRounding.AwayFromZero`. 11 x 5.15 -> 50.985 gives 50.98 instead of 50.99 |
| SHW-103 | `Program.cs` `GET /books/{id}` | returns `Results.Ok(null)` instead of throwing/returning 404 |
| SHW-104 | `OrderService.CancelAsync` | never calls `InventoryService.ReleaseAsync` (which exists and is unused); cancelling twice must not restock twice |
| SHW-105 | `OrderService.PlaceOrderAsync` | reserves stock line by line with no rollback when a later line fails |
| SHW-106 | `InventoryService.ReserveAsync` | check-then-act on a shared mutable `Book` with no synchronisation; the in-memory repo hands out the same instance to all callers |
| SHW-107 | `InventoryService.ReserveAsync` / `PricingService` | negative quantity passes the stock check and *increases* stock, then `PriceLine` throws `ArgumentOutOfRangeException` -> 500 with stock already corrupted. Quantity 0 -> 500 with no corruption |
| SHW-301 | `CatalogService.AddAsync` | `DateTime.Now` (local) vs `DateTime.UtcNow` in `OrderService` |

Missing features / gaps (no code is "wrong", the capability is absent): book validation
(201), search/paging (202), status state machine (203: ship a cancelled order works today),
discount codes (204), low-stock report (205), order listing (206), idempotency (207),
persistence (304), auth (501), rate limiting (502), request logging (402), OpenAPI (403).

Other things a good reviewer (SHW-602) could legitimately raise, **not** ticketed:
- `Order`/`Book` are mutable and shared by reference with the repositories, so callers can mutate stored state without calling `UpdateAsync`
- `CustomerEmail` is never validated
- Duplicate `BookId` lines in one order are priced separately, so the bulk discount can be dodged by splitting a purchase across lines (e.g. 2 lines of 6)
- `Results.Created` paths are hard-coded strings
- `ListAsync` on the order repository sorts by `CreatedAt` only; ties come back in arbitrary order (dictionary enumeration)
- `Quantity` has no upper bound, and the exception middleware doesn't catch anything but the three domain exception types, so any other exception is an unformatted 500

Not verified by compiler: the C# in this repo was written in an environment without the
.NET SDK. If the first `dotnet build` fails, it is a typo or a package-version issue, not a
planted defect. Fix it before starting any experiment (and re-tag `baseline`).
