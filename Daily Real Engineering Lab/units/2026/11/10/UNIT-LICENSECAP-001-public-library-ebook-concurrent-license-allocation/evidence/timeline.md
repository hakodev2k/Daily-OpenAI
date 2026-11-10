# Incident evidence — dữ liệu giả lập cố định

## Timeline A: hot ISBN BK-41 (2 license)
| Tick | Instance | Operation | Read active | Action recorded | HTTP |
|---|---|---|---:|---|---|
| T0 | — | seed | 1 | loan L100 active | — |
| T1 | API-A | op-A patron P1 | 1 | checks free slot | — |
| T2 | API-B | op-B patron P2 | 1 | checks free slot | — |
| T3 | API-A | op-A | — | inserts loan L101 active | 200 GRANTED |
| T4 | API-B | op-B | — | inserts loan L102 active | 200 GRANTED |
| T5 | audit | count BK-41 | 3 | contract limit 2 | alert |

## Timeline B: response lost after external side effect
| Tick | Component | Event |
|---|---|---|
| T0 | client | sends op-C for BK-52 |
| T1 | API-A | creates loan L201; sends DRM grant |
| T2 | DRM | accepts entitlement E201 |
| T3 | network | API response lost; client times out |
| T4 | client | retries op-C to API-B |
| T5 | API-B | creates loan L202; sends second grant |
| T6 | DRM | accepts E202 |
| T7 | audit | two loans and two entitlements for one intended operation |

## Distractors
- CPU 21%, memory steady.
- Database p95 18ms; no deadlock alarms.
- CDN cache hit rate 94% (unrelated to loan writes).
- Retry volume increases 3× during a mobile carrier outage.

## Tasks
1. Enumerate possible serial orderings that should be valid.
2. Identify the earliest point where a decision becomes unsafe.
3. Separate database commit, DRM grant and HTTP response as distinct events.
4. State what evidence would falsify your leading hypothesis.
