# After — acceptance contract

```text
ReceiptCount=2; TotalWeightKg=300; AverageWeightKg=150; SnapshotOpens=1
LiveConsistency=PASS; Empty=PASS; Stable=PASS
```

`verify.ps1` trả exit code 0. Trên feed rỗng: count=0, total=0, average=0. Trên feed ổn định: count=2, total=4.00, average=2.00. Không có external side effects.
