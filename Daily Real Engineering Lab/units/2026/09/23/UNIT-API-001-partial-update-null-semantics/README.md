# UNIT-API-001 — Partial Update: Null Semantics

## Mục tiêu
Điều tra một API cập nhật hồ sơ nơi request hợp lệ nhưng một số field bị thay đổi ngoài ý muốn.

## Bối cảnh thực tế
Một endpoint PATCH-like nhận JSON để cập nhật hồ sơ khách hàng. Sau một release, support ghi nhận request chỉ đổi `displayName` đôi khi làm mất `phoneNumber` đã có.

## Bạn cần làm gì
1. Chạy starter và reproduce.
2. Ghi hypothesis trước khi sửa.
3. Xác định contract nào bị mất giữa JSON payload, DTO và update logic.
4. Sửa starter mà không làm hỏng khả năng chủ động xóa số điện thoại.
5. Chạy verify.
6. Sau đó mới xem solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy hai request mô phỏng trên cùng dữ liệu: một request chỉ đổi tên và một request có chủ đích thay đổi phone. Quan sát state trước/sau và assertion.

## Những gì cần quan sát
- Field nào xuất hiện trong payload.
- Giá trị DTO sau deserialize.
- State persisted sau update.
- Hai business intents khác nhau có bị biểu diễn thành cùng một runtime state hay không.

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
[Reference Solution — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
Trước fix, request chỉ đổi tên làm thay đổi thêm một field không có trong payload. Sau fix, omitted field giữ nguyên; explicit update vẫn hoạt động theo contract đã định nghĩa.

## Estimated Time
35–55 phút
