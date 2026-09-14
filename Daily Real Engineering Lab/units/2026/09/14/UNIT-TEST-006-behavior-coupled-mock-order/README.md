# UNIT-TEST-006 — Behavior-Coupled Mock Order

## Mục tiêu

Thực hành nhận diện một test suite đang cản trở refactoring dù hành vi nghiệp vụ quan sát được vẫn đúng, sau đó cải thiện test để bảo vệ contract quan trọng thay vì khóa chặt chi tiết triển khai.

## Bối cảnh thực tế

Một service gửi notification vừa được refactor nội bộ để chuẩn bị tách telemetry. Sau thay đổi, notification vẫn được gửi đúng, audit vẫn được ghi đúng và kết quả trả về không đổi, nhưng CI đỏ vì một unit test cũ.

Team không muốn đơn giản xóa test hoặc sửa production code chỉ để làm test cũ xanh. Bạn cần xác định test đang bảo vệ điều gì, phần nào thực sự là contract nghiệp vụ và phần nào chỉ là coupling với implementation.

## Bạn cần làm gì

1. Chạy reproduction và đọc failure message.
2. Kiểm tra production behavior trước khi kết luận refactor bị lỗi.
3. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
4. Chỉnh test trong `starter/` để vẫn bắt được regression thực tế nhưng không phụ thuộc vào chi tiết không thuộc contract.
5. Chạy `verify.ps1`.
6. Sau khi tự sửa, so sánh với reference solution.

Không sửa production code trừ khi bạn có bằng chứng cho thấy business behavior thực sự sai.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Internet ở lần restore NuGet đầu tiên

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-TEST-006-behavior-coupled-mock-order"
./reproduce.ps1
```

Sau khi chỉnh `starter/NotificationCoordinatorTests.cs`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` restore project, chạy test suite ở trạng thái starter và chỉ thành công khi quan sát được failure dự kiến.

## Những gì cần quan sát

- Business result trả về từ coordinator.
- Số notification thực sự được gửi.
- Số audit record được ghi.
- Assertion nào làm test fail.
- Assertion đó đang bảo vệ observable contract hay một chi tiết nội bộ.

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

> Reference Solution — chỉ xem sau khi reproduce vấn đề và thử fix của riêng bạn.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Business assertions pass.
- Một assertion liên quan đến interaction sequence làm test fail.
- Notification và audit side effects vẫn có số lượng đúng.

After:
- Test suite pass.
- Test vẫn phát hiện được các regression như không gửi notification, gửi sai payload hoặc bỏ audit.
- Một refactor chỉ thay đổi thứ tự nội bộ không cần thiết không làm test đỏ.

## Estimated Time

35–50 phút.
