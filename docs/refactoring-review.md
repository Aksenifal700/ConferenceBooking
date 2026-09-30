# Final review — 2026-09-30

- Business failures use NotFoundException (404), ConflictException (409), BadRequestException (400), and InvalidCredentialsException (401).
- Invalid JWT configuration fails startup with InvalidOperationException. Calculator argument guards retain standard exceptions; HTTP input is validated before calculation.
- Repositories return explicit outcomes for booking conflicts and archiving. PostgreSQL errors other than the known exclusion constraint are not disguised as client errors.
- Room archiving and booking insertion lock the same room row. The user confirmed concurrent booking and archive scenarios were checked.
- Identity uses user-only stores; roles and role seeding are absent from runtime code. RemoveUnusedRoleTables removes historical role tables when applied. It has been verified on a temporary clean database, not applied to the existing working database by this review.
- Reports were checked through HTTP on PostgreSQL: successful requests, invalid periods, and missing authorization.
- All six migrations were applied to a temporary clean database: three rooms, nine service links, no role tables, and the booking exclusion constraint.
- There is no automated test project, as requested. README documents the manual checks and simplified authorization model.