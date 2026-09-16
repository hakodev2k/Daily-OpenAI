# UNIT-REDIS-003 — Session Cache Expires While It Is Being Refreshed

## Mục tiêu
Điều tra một cache workflow có vẻ hoạt động đúng ở từng thao tác riêng lẻ nhưng đôi khi làm mất session đang hoạt động khi refresh và expiry xảy ra gần nhau.

## Bối cảnh thực tế
Một backend dùng Redis-like cache để giữ session snapshot. Request đọc session, cập nhật payload rồi gia hạn TTL. Trong log production, một số session vừa được cập nhật thành công nhưng biến mất ngay sau đó. Tỷ lệ thấp, khó tái hiện bằng test tuần tự.

## Bạn cần làm gì
1. Chạy starter để reproduce interleaving đã được cố định.
2. Ghi lại timeline của hai actor và trạng thái key sau mỗi bước.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Xác định invariant mà refresh operation phải bảo vệ.
5. Sửa `starter/` để update và TTL transition không tạo cửa sổ làm mất session hợp lệ.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy deterministic in-memory Redis simulator với một interleaving cố định giữa refresh và expiry worker.

## Những gì cần quan sát
- Version/value của key tại từng event.
- Actor nào nhìn thấy state nào.
- Kết quả cuối cùng có giữ session vừa refresh hay không.
- Không dựa vào timing ngẫu nhiên; timeline được điều khiển bằng step barriers.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** timeline kết thúc với key không tồn tại dù refresh operation đã báo thành công.

**After:** cùng interleaving không thể xóa state mới hơn; payload mới tồn tại với TTL hợp lệ.

## Estimated Time
40–60 phút.