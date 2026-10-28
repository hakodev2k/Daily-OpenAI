# Sau sửa — learner-edited starter

```text
first_snapshot_count_after_next_scan=1 (expected 1)
second_snapshot_count_after_reader_attempt=2 (expected 2)
reader_could_mutate_snapshot=False (expected False)
active_manifest_unchanged_by_reader=True (expected True)
timestamps_preserved=True (expected True)
other_flight_isolated=True (expected True)
verification_passed=True
```

`./verify.ps1` phải thành công trên code đã sửa trong **starter/**. Để kiểm tra reference riêng biệt: `dotnet run --project ./solution/ManifestAudit.csproj -- verify`. Hai đường kiểm tra không được đánh đồng.
