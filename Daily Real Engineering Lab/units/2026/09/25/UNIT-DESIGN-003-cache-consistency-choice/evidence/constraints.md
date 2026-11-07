# Constraints

- Peak read: 8,000 req/s.
- Normal writes: 50/s; burst writes: 1,500/s for 20 seconds.
- Read p95 target: 120 ms.
- Availability read may be stale up to 10 seconds.
- Checkout commit must revalidate authoritative stock.
- Primary database has measured safe read budget of 2,500 req/s for this workload.
- Team can operate one additional managed data service, but prefers fewer moving parts.
