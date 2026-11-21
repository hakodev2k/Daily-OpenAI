# Senior extension
Thiết kế unique key `(tenantId, billingPeriod)` cho hai replicas, mô phỏng crash sau commit trước ACK, retry và lease expiry. Phân biệt single-flight local với exactly-once effect.
