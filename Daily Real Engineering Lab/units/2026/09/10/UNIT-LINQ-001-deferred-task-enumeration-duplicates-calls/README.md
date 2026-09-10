# UNIT-LINQ-001 — Deferred Task Enumeration Duplicates Calls

## Mục tiêu

Điều tra một batch processor có kết quả nghiệp vụ đúng nhưng số lần gọi downstream lại cao hơn số item đầu vào. Mục tiêu là dùng evidence để xác định vì sao cùng một batch có thể phát sinh side effect ngoài dự kiến và sửa mà không làm thay đổi contract xử lý song song.

## Bối cảnh thực tế

Một background worker nhận 3 invoice và gọi payment provider để capture từng invoice. Dashboard downstream cho thấy mỗi batch 3 invoice đôi lúc tạo nhiều hơn 3 request, dù worker chỉ báo 3 receipt thành công. Đây là rủi ro nghiêm trọng nếu downstream API không idempotent.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis trước khi xem hints.
3. Tìm nguyên nhân khiến số downstream call lớn hơn số invoice.
4. Sửa trực tiếp code trong `starter/`.
5. Chạy `verify.ps1` để chứng minh mỗi invoice chỉ gây đúng một downstream call và vẫn xử lý đủ 3 receipt.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay dịch vụ ngoài

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ chạy starter và xác nhận rằng batch gồm 3 invoice nhưng downstream provider nhận nhiều hơn 3 call.

## Những gì cần quan sát

- `processed` vẫn bằng `3`.
- `providerCalls` lớn hơn số invoice đầu vào.
- Không có exception bắt buộc phải xuất hiện.
- Hãy chú ý thời điểm từng request downstream được tạo ra so với các lần code duyệt qua pipeline xử lý.

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

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- `processed=3`
- `providerCalls=6`
- `reproduce.ps1` xác nhận symptom tồn tại

After:
- `processed=3`
- `providerCalls=3`
- `verify.ps1` pass

## Estimated Time

30–45 phút.
