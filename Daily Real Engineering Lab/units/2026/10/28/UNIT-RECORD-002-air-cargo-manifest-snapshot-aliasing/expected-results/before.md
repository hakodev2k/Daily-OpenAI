# Trước sửa — original starter

```text
first_snapshot_count_after_next_scan=2 (expected 1)
second_snapshot_count_after_reader_attempt=3 (expected 2)
reader_could_mutate_snapshot=True (expected False)
active_manifest_unchanged_by_reader=False (expected True)
timestamps_preserved=True (expected True)
other_flight_isolated=True (expected True)
verification_passed=False
```

`./reproduce.ps1` trả thành công khi quan sát đúng lỗi; `./verify.ps1` phải thất bại trên starter chưa sửa. Đây là số liệu deterministic từ fixture, không phải benchmark.
