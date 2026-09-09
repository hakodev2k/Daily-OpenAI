# UNIT-CS-005 — Valid Data Disappears at the Boundary

## Mục tiêu

Điều tra một lỗi correctness trong pipeline LINQ nơi dữ liệu hợp lệ bị coi là “không tồn tại”, xác định bằng evidence thay vì đoán, rồi sửa contract để phân biệt rõ presence và value.

## Bối cảnh thực tế

Một service tính phí giao dịch chọn mức phí override theo merchant. Production ghi nhận một merchant có cấu hình override hợp lệ nhưng API lại trả về mức phí mặc định. Không có exception, log truy vấn vẫn cho thấy record tồn tại và unit test với các mức phí dương đều pass.

Business impact: một nhóm merchant bị tính sai phí dù dữ liệu cấu hình không thiếu.

## Bạn cần làm gì

1. Chạy starter và ghi lại input, dữ liệu tìm thấy và kết quả cuối cùng.
2. Viết ít nhất hai hypothesis trước khi xem hints.
3. Xác định tại boundary nào thông tin “có record hay không” bị mất.
4. Sửa code trong `starter/` mà không thay đổi dataset để che lỗi.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần database, Docker hoặc external service

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Reproduction hợp lệ phải chứng minh rằng một merchant có record cấu hình nhưng kết quả business lại giống trường hợp không có record.

## Những gì cần quan sát

- số lượng record match
- raw value của record match
- value trả về từ bước lookup
- nhánh fallback có chạy hay không
- liệu cùng một value có đang đại diện cho hai trạng thái business khác nhau

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

> **Spoiler:** chỉ mở sau khi bạn đã tự reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- merchant `M-ZERO` có đúng một override record
- override value là dữ liệu hợp lệ
- service vẫn trả về fallback value

Sau khi sửa:

- `M-ZERO` giữ nguyên override hợp lệ
- merchant không có override vẫn dùng fallback
- verification kiểm tra cả hai trường hợp để tránh fix accidental

## Estimated Time

30–45 phút.
