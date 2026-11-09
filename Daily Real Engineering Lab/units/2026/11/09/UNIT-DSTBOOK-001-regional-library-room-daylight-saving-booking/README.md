# UNIT-DSTBOOK-001 — Đặt phòng thư viện vào ngày đổi giờ mùa hè

## Mục tiêu
Điều tra một incident lịch đặt phòng, phân biệt giờ địa phương với thời điểm UTC và bảo vệ hợp đồng đặt chỗ.

## Bối cảnh thực tế
Thư viện khu vực cho phép đặt phòng theo giờ địa phương của chi nhánh. Trong hai ngày chuyển giờ tại New York, nhân viên báo cáo một số lượt đặt phòng xuất hiện lệch giờ hoặc được xác nhận dù không có khung giờ tương ứng. Các ngày bình thường vẫn hoạt động.

## Bạn cần làm gì
Reproduce bằng fixture cố định; ghi ít nhất ba giả thuyết; thu thập kết quả cho ngày thường, ngày chuyển sang giờ mùa hè và ngày quay về giờ tiêu chuẩn; sửa code trong `starter/` rồi verify.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+; không cần database, Azure hoặc Internet sau khi cài SDK.

## Chạy nhanh
Từ thư mục unit: `pwsh ./scripts/setup.ps1`, `pwsh ./scripts/run.ps1`.

## Cách reproduce vấn đề
`pwsh ./scripts/reproduce.ps1` chạy fixture trên starter nguyên bản và xác nhận incident còn tồn tại. Exit code 0 nghĩa là tái hiện được lỗi; khác 0 nghĩa là fixture không còn thể hiện đúng sự cố.

## Những gì cần quan sát
So sánh kết quả UTC và mã trạng thái cho các local timestamps: 2026-03-08 02:30, 2026-11-01 01:30 và 2026-11-02 10:00 ở America/New_York. Ghi lại cả offset nếu có.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `pwsh ./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [lời giải](solution/README.md) sau khi tự điều tra.

## Expected Results
Trước: fixture incident được phát hiện. Sau: ngày bình thường giữ đúng UTC; giờ không tồn tại bị từ chối; giờ trùng cần chỉ rõ offset và hai lựa chọn tạo hai UTC khác nhau.

## Estimated Time
60–75 phút.
