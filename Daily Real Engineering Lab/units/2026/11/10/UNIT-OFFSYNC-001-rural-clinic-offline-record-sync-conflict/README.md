# UNIT-OFFSYNC-001 — Đồng bộ hồ sơ khám ngoại tuyến sau mất kết nối

## Mục tiêu
Đánh giá thiết kế đồng bộ dữ liệu có xung đột, chứng minh quyết định bằng replay và xác định ranh giới an toàn dữ liệu.

## Bối cảnh thực tế
Hai máy tính bảng tại phòng khám vùng sâu ghi chú cùng một lượt khám khi mất mạng. Sau khi kết nối trở lại, dashboard hiển thị một số thông tin không khớp nhật ký thao tác.

## Bạn cần làm gì
1. Đọc `evidence/replay.csv`, lập bảng trạng thái sau từng sự kiện.
2. Tái hiện kết quả với chính sách đồng bộ hiện hành mô tả tại `docs/scenario.md`.
3. Nêu ít nhất ba hypothesis và xác định chứng cứ phân biệt.
4. Đề xuất hợp đồng API, state machine, quy tắc xử lý conflict và rollback.
5. Đối chiếu tiêu chí tại `expected-results/acceptance.md` trước khi mở lời giải.

## Yêu cầu môi trường
Không cần cloud hay dữ liệu bệnh nhân thật; dùng PowerShell 7 hoặc trình đọc CSV/Markdown bất kỳ.

## Chạy nhanh
```powershell
Import-Csv ./evidence/replay.csv | Format-Table
```

## Cách reproduce vấn đề
Áp dụng từng hàng theo thứ tự `arrivalOrder`, sử dụng chính sách được mô tả trong scenario; ghi lại các field cuối cùng của lượt khám V-204.

## Những gì cần quan sát
So sánh dữ liệu hiển thị, lịch sử chỉnh sửa và thứ tự nhận sự kiện. Ghi lại sự kiện làm thay đổi nội dung quan trọng và liệu thao tác đó có được người dùng xác nhận hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix / thiết kế.
4. Verify với toàn bộ replay.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Mở sau khi hoàn tất](solution/README.md).

## Expected Results
[Acceptance và kiểm thử](expected-results/acceptance.md)

## Estimated Time
55–75 phút.
