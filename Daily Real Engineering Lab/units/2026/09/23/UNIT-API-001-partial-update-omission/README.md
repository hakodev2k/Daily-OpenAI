# UNIT-API-001 — Partial Update: Omitted Field Becomes a Real Change

## Mục tiêu
Điều tra một API cập nhật một phần resource nhưng đôi khi thay đổi cả preference mà client không gửi lên.

## Bối cảnh thực tế
Mobile app chỉ muốn đổi `displayName` của customer. Sau request thành công, một số customer phát hiện tùy chọn nhận newsletter bị tắt dù UI không hề gửi thay đổi đó. Request không lỗi, response trông hợp lệ và bug chỉ xuất hiện với một số payload partial-update.

## Bạn cần làm gì
1. Chạy starter và reproduce.
2. Quan sát state trước/sau cho payload chỉ chứa `displayName`.
3. Ghi ít nhất 2 hypothesis trước khi sửa.
4. Sửa contract/update path để field không được gửi không trở thành thay đổi ngoài ý muốn.
5. Chạy verify và đảm bảo client vẫn có thể chủ động gửi `false`.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy scenario với customer ban đầu có newsletter bật và payload chỉ đổi tên.

## Những gì cần quan sát
- Giá trị `newsletterEnabled` trước request.
- Payload JSON thực tế.
- Giá trị sau update.
- Scenario thứ hai nơi client chủ động gửi `newsletterEnabled: false`.

Không chỉ sửa để một test xanh; contract phải phân biệt đúng intent của hai request trên.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints/hint-01.md`, sau đó `hint-02.md`, `hint-03.md` nếu cần.

## Reference Solution
`solution/README.md` — spoiler: chỉ xem sau khi đã tự sửa.

## Expected Results
Trước khi sửa, request chỉ đổi tên làm một preference không liên quan thay đổi. Sau khi sửa, field bị omit phải giữ nguyên; explicit `false` vẫn phải được áp dụng.

## Estimated Time
45 phút.
