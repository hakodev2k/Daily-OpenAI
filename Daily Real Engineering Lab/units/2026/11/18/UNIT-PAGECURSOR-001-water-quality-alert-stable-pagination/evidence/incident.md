# Incident evidence (synthetic, deterministic)
- 10:00:00: dashboard requests page 0 size 3; six alerts present.
- 10:00:01: device submits alert 107 with newer timestamp.
- 10:00:02: dashboard requests page 1 size 3.
- HTTP 200 both requests; CPU 14%, p95 35 ms (noise, not evidence of a slow API).
- Operator reports 103 appears twice and 106 not reviewed.
- Two alerts have identical ObservedAt; tie-order is not specified in API contract.

## Investigation questions
Which invariant is violated? Which evidence differentiates duplicate submission from page-boundary drift? What consistency guarantee should the API expose?
