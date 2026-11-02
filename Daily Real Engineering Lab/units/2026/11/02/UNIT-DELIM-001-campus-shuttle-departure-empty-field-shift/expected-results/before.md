# Before — baseline

- `./scripts/reproduce.ps1` trả về thành công **vì** baseline có ít nhất một `CONTRACT_FAIL` và process kiểm tra trả mã 1.
- `./scripts/run.ps1` cho thấy bản ghi có ghi chú hoạt động bình thường, bản ghi ghi chú rỗng có dữ liệu hiển thị không đúng vị trí.
- `./scripts/verify.ps1` phải thất bại trên starter chưa sửa; đây là hành vi đúng của quality gate.
- Nếu build thất bại, đó là lỗi môi trường, **không** được coi là reproduce thành công.
