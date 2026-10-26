# ADR — Search index v2 migration

## Trạng thái

Proposed / Accepted / Rejected: ...

## Triệu chứng và evidence

- Timeline:
- Query impact:
- Những giả thuyết cần kiểm chứng:

## Business constraints và bất biến

- SLO:
- Data correctness:
- Security:
- Budget:
- Rollback:

## Phép tính

- Backfill:
- CDC backlog:
- Net replay:
- Catch-up:
- Storage peak:
- Điều kiện campaign surge:

## Phương án

| Phương án | Availability | Freshness | Rollback | Cost | Risks |
|---|---|---|---|---|---|
| A | | | | | |
| B | | | | | |
| C | | | | | |

## Quyết định và lý do

...

## Data flow và ordering

```text
source -> ... -> query
```

## Runbook

1. Chuẩn bị:
2. Backfill:
3. Catch-up:
4. Shadow validation:
5. Canary:
6. Cutover:
7. Rollback:
8. Cleanup:

## Acceptance gates

- Correctness:
- p95:
- Availability:
- Freshness:
- Replay lag:
- Security:
- Rollback drill:

## Owner, telemetry và cảnh báo

...

## Requirement change: 600 events/s trong 4 giờ

- Phép tính:
- Quyết định thay đổi:
- Điều kiện để tiếp tục:
