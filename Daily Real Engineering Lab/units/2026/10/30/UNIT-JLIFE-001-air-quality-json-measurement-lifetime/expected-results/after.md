# After — acceptance

Sau khi tự sửa `starter/`, chạy `./verify.ps1`.

- Cả hai station trả đúng `StationId`, `Pm25`, `Unit`.
- `PUBLISHED=2/2`, `LAB_RESULT=PASS`, `VERIFIED=YES`.
- JSON sai cú pháp tiếp tục bị từ chối qua `JsonException`.
- `verify.ps1` trả exit code 0; không dùng code trong `solution/` để đánh giá learner.

Regression: thêm station thứ ba và nested metrics khác để tránh hard-code theo input.
