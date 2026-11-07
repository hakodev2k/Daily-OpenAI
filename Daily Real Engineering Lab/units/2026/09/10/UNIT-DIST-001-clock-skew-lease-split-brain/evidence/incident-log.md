# Incident Evidence

```text
04:00:00Z storage  partition=invoice-2026-09-10 owner=worker-a lease-acquired
04:00:05Z worker-a batch=invoice-2026-09-10 status=processing
04:00:05Z worker-b local-clock=04:00:45Z attempt=takeover
04:00:05Z storage  partition=invoice-2026-09-10 owner=worker-b lease-updated
04:00:06Z worker-b batch=invoice-2026-09-10 status=processing
04:00:08Z metrics cpu=31% memory=44% queue-depth=12 db-latency=normal
04:00:20Z worker-a batch=invoice-2026-09-10 status=completed
```

Một vài observation phụ:

- Không có retry storm.
- Không có process restart trong incident window.
- Network latency giữa worker và storage vẫn trong baseline.
- Hai worker chạy trên hai node khác nhau.
