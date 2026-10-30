# Incident evidence — fixture giả lập, không phải production logs

```text
11:22:09.120 request permit=PERMIT-101 tenant=TENANT-A auth=valid accepted=true
11:22:09.125 request permit=PERMIT-202 tenant=TENANT-B auth=valid accepted=true
11:22:09.128 queue pending=2 worker_threads=1
11:22:10.008 worker audit permit=PERMIT-101 failed=AUDIT_TENANT_CONTEXT_MISSING
11:22:10.010 metrics http_accepted=2 audit_persisted=0 cpu_percent=9
11:22:10.012 rollout replicas=3 (unchanged from previous day)
```

Log được dựng để mô phỏng incident. Đừng coi HTTP accepted là bằng chứng audit đã lưu. Dùng contract thực tế để xác nhận giả thuyết.
