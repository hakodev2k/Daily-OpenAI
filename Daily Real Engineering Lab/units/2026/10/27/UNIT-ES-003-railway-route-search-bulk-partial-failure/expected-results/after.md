# After
- `verify.ps1` in `VERIFIED_LEARNER_STARTER`.
- `INDEXED_REPORTED` và `INDEXED_ACTUAL` đều bằng `route-101,route-102,route-104`.
- `QUARANTINED=route-103`, `RETRY_EXHAUSTED=` rỗng.
- `ATTEMPTS=route-101:1,route-102:2,route-103:1,route-104:1`.
- `CHECK_PASSED` xác nhận từng ID được xử lý, không retry lại action đã thành công và không báo thành công giả.
- Nếu không reproduce được: chạy `./setup.ps1`, kiểm tra SDK có target `net8.0`, không sửa `starter/Scenario.cs`, kiểm tra output build trước khi kết luận.
