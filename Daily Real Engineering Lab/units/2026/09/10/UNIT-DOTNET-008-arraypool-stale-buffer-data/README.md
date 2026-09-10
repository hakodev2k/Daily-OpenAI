# UNIT-DOTNET-008 — Dữ liệu request trước xuất hiện trong buffer của request sau

## Mục tiêu

Điều tra một lỗi data isolation trong worker xử lý nội dung nhạy cảm, nơi request sau quan sát byte mà chính nó chưa từng ghi.

## Bối cảnh thực tế

Một document-processing worker tái sử dụng buffer để giảm allocation. Sau khi xử lý tài liệu của tenant A, một request của tenant B đôi khi thấy fragment dữ liệu cũ trong vùng buffer trước khi payload mới được ghi đầy đủ. GC và application cache không cho thấy dấu hiệu bất thường.

## Bạn cần làm gì

1. Reproduce hiện tượng bằng starter.
2. Ghi lại evidence và ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
3. Xác định lifecycle nào cho phép dữ liệu cũ xuất hiện lại.
4. Sửa `starter/` để tenant B không thể quan sát nội dung nhạy cảm của tenant A.
5. Chạy `verify.ps1` và bảo đảm chức năng xử lý payload vẫn đúng.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Tenant A ghi một payload có marker `ALPHA-42`.
- Buffer được trả về sau khi request A hoàn thành.
- Tenant B nhận một buffer cho request độc lập.
- Trước khi B ghi payload đầy đủ, snapshot có thể chứa marker của A.
- Không có biến static nào chủ động copy payload A sang B.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước fix, reproduction phải phát hiện marker cũ trong buffer của request B. Sau fix, `verify.ps1` phải xác nhận marker không còn xuất hiện và payload của B vẫn được xử lý đúng.

Nếu máy của bạn không reproduce được, chạy lại vài lần và giữ nguyên `ArrayPool<byte>.Create(64, 1)` trong starter; cấu hình này cố ý giới hạn pool để làm hiện tượng ổn định hơn.

## Estimated Time

30–45 phút.
