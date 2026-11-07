# UNIT-READINESS-001 — Mobile clinic: probe policy gây restart storm

## Mục tiêu
Điều tra sự khác biệt giữa tình trạng process, khả năng phục vụ request và quyết định restart khi dependency gián đoạn.

## Bối cảnh thực tế
Gateway đặt lịch của phòng khám lưu động chạy trên Kubernetes. Trong lúc PostgreSQL mất kết nối ngắn, các pod liên tục khởi động lại, request mới bị từ chối và một số tác vụ đang xử lý biến mất. Nhóm vận hành cần chọn chính sách probe phù hợp thay vì chỉ tăng số replica.

## Bạn cần làm gì
Chạy mô phỏng, ghi số restart, rejected request và lost in-flight work. Đề xuất một probe policy có thể bảo vệ cả khả năng phục hồi và dữ liệu đang xử lý; sửa code trong `starter/Program.cs` và kiểm chứng.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+ (Windows); không cần Kubernetes, database hay cloud account.

## Chạy nhanh
```powershell
pwsh ./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy từ thư mục unit. Mô phỏng 4 phút với database gián đoạn ở phút 1–2. `reproduce.ps1` phải xác nhận trạng thái chưa đạt yêu cầu vận hành.

## Những gì cần quan sát
So sánh các dòng `minute=` với `SUMMARY`; phân biệt lỗi phục vụ traffic với sự cố cần restart process. Ghi lại giả thuyết trước khi sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `pwsh ./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md) — chỉ xem sau khi tự thử.

## Expected Results
Trước: outage gây restart và mất in-flight work. Sau: không restart process còn sống chỉ vì database tạm thời unavailable; không nhận traffic khi không thể phục vụ; khi dependency phục hồi có thể nhận lại traffic. [Chi tiết](expected-results.md).

## Estimated Time
35–50 phút.
