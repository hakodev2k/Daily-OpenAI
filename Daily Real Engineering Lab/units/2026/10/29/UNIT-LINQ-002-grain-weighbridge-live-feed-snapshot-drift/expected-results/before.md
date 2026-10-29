# Before — starter chưa sửa

```text
ReceiptCount=2; TotalWeightKg=700; AverageWeightKg=350; SnapshotOpens=2
REPRODUCED: inconsistent live report
```

`reproduce.ps1` trả 0 khi thấy symptom; `verify.ps1` trả nonzero vì live consistency và one-read contract chưa đạt. Số liệu này chỉ dành cho fixture synthetic, không phải telemetry production.
