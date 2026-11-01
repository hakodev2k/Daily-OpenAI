# After — hợp đồng cần đạt

- `verify` chạy trên `starter/Program.cs` đã sửa, exit code 0.
- Tất cả 13 case PASS; `SUMMARY: passed=13/13; originalSymptom=False; allPassed=True`.
- Cả HTTP-date tương lai/quá khứ, delta-seconds, malformed/missing header, budget boundary và status không retry đều được kiểm tra.
- Không cần sleep, không phụ thuộc timezone hệ điều hành hay network.
