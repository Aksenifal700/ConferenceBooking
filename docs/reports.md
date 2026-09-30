# Reports

All endpoints require a valid JWT. With the project's role-free authorization, every authenticated user can access business reports.

Query parameters: `from=2026-10-01&to=2026-11-01`. Dates are UTC; from is inclusive and to is exclusive. The period must contain 1–366 days. Missing, reversed and excessive periods are rejected. Date-only parameters cover complete days.

- GET /api/reports/revenue: count and sum of stored booking totals per room, selected by booking start date. Includes selected services. This is booked value, not payments received. Future bookings are included when within the selected period.
- GET /api/reports/occupancy: booked hours intersecting the period divided by 17 operating hours per day (06:00–23:00 UTC), multiplied by 100. Values are rounded to two decimals after calculation.
- GET /api/reports/services: number of bookings containing a service and sum of its stored prices, selected by booking start date. Grouped by service ID and historical name; a renamed service can have separate rows. Unused services are omitted.

Room reports include zero-booking rooms and archived rooms, marked with isArchived. Room names are current; service names and prices are booking snapshots. Occupancy uses the full calendar period for every room: creation/archive timestamps are not stored, so this is calendar occupancy, not historical availability-adjusted occupancy.

No payment, cancellation, export, or role system is introduced. These three reports are an implementation choice for the assessment's open-ended analytics requirement.
