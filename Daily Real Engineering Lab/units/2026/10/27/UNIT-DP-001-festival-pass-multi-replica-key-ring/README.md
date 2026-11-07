# UNIT-DP-001 — Festival Pass: phiên đăng nhập mất hiệu lực khi chuyển replica

## Mục tiêu

Điều tra sự cố xác thực của một hệ thống ASP.NET Core chạy nhiều replica, tái hiện độc lập qua các process, thu thập bằng chứng và sửa cấu hình bảo vệ token mà không làm suy yếu ranh giới bảo mật.

## Bối cảnh thực tế

Cổng vé lễ hội phục vụ hàng nghìn lượt quét mã. Sau khi tăng số replica từ một lên hai, một số khách hàng nhận thông báo phiên không hợp lệ dù vừa đăng nhập. Khi rolling restart, tỷ lệ lỗi tăng trong vài phút. Database vé và hệ thống phát hành đều báo healthy.

Môi trường lab mô phỏng lớp bảo vệ session token bằng ASP.NET Core Data Protection. Không cần Kubernetes, database, network hay dịch vụ trả phí.

## Bạn cần làm gì

1. Chạy harness và ghi lại sự khác biệt giữa mint/read ở các process và replica.
2. Đưa ra ít nhất ba hypothesis từ triệu chứng và evidence trước khi mở hints.
3. Chỉnh **chỉ** `starter/KeyRingFactory.cs`; giữ nguyên purpose của token.
4. Chạy `verify.ps1`; kiểm tra cả khả năng chia sẻ phiên và hai negative security cases.
5. So sánh với reference solution và đề xuất kế hoạch rollout, bảo vệ key at rest và thu hồi key.

## Yêu cầu môi trường

- .NET SDK 10 theo `global.json`; PowerShell 5.1+ hoặc 7+.
- Không có NuGet package bên ngoài; sử dụng shared framework `Microsoft.AspNetCore.App`.
- Thư mục key-ring được tạo tạm trong máy và xóa sau mỗi kịch bản.

## Chạy nhanh

Từ thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi sửa `starter/KeyRingFactory.cs`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` phát hành một token ở process A rồi kiểm tra token tại process B và tại một process A mới. Script chỉ trả exit code 0 khi **đã tái hiện đúng sự cố dự kiến**; đó không phải dấu hiệu code đã được sửa.

## Những gì cần quan sát

- Token do process A phát hành có đọc lại được ở process B không?
- Process A mới, cùng tên replica nhưng PID khác, có đọc được không?
- Các lệnh chạy riêng process, nên state nào có thể và không thể được chia sẻ?
- Negative cases: purpose khác hoặc thư mục key-ring khác phải luôn bị từ chối.
- Kết quả không phụ thuộc timing hay random network failure.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi đã tự sửa và chạy verification.

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

75 phút (L4 refactoring, D4). Ghi hypothesis và quyết định kỹ thuật trong [workspace](workspace/my-investigation.md).
