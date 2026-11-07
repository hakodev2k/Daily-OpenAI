# After — acceptance targets

- Tổng: 224 succeeded, 8 failed, 7 offline, 1 in progress; 232 terminal. Failure rate 8/232 ≈ 3,45% > 2%, NO-GO cho wave tiếp theo trước triage.
- Durable device state có attempt fencing và idempotent ACK ingestion; stale/duplicate không thay đổi terminal counts.
- Global cap 240, site cap 1, site availability 3/4; chứng minh bằng atomic admission.
- Pause chặn **new commands** trong 5 phút, không hứa rollback ngay với offline/in-flight.
- Dual-bank rollback chỉ thành công sau health-check ACK; single-bank failure cần manual intervention.
- Capacity math xét 45 MiB full, 12 MiB delta, 80% eligible, 320 GiB/day và offline.
- ADR có alternatives, security, observability, A1–A6 walkthrough.

Đây là acceptance criteria cho thiết kế, không phải production implementation.
