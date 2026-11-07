# After — learner starter

- `./scripts/verify.ps1` phải in đúng 7 dòng `CONTRACT_PASS`, `SUMMARY total=7 failed=0` và exit code 0.
- `SH-210|P4||North Terminal|BOARDING` giữ `note` rỗng, `destination=North Terminal`, `status=BOARDING`.
- `SH-211|P2|Gate C|Harbor Museum|DELAYED` vẫn hợp lệ.
- Bản ghi thiếu/thừa trường hoặc trường bắt buộc rỗng phải bị từ chối bằng `FormatException`.
- `./scripts/reproduce.ps1` vẫn tái hiện baseline bất biến sau khi starter được sửa.
