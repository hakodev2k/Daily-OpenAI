# UNIT-DOTNET-010 — JSON Web Defaults Drift

## Mục tiêu

Điều tra một background worker đọc payload JSON hợp lệ nhưng tạo object với một số field quan trọng bị mất giá trị, dù cùng payload đó hoạt động đúng khi đi qua ASP.NET Core HTTP endpoint.

## Bối cảnh thực tế

Một hệ thống notification vừa tách logic xử lý khỏi API sang background worker. API cũ nhận payload từ web client và gửi email bình thường. Worker mới đọc chính payload JSON đó từ file/message body, báo `processed=1` nhưng không tạo notification. Không có exception deserialization.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại evidence và ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
3. Xác định vì sao cùng shape JSON lại có hành vi khác nhau giữa hai execution path.
4. Sửa trực tiếp code trong `starter/`.
5. Chạy `verify.ps1` để chứng minh notification được tạo mà không hard-code dữ liệu input.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Không cần database, Docker hoặc external service

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script build và chạy starter, sau đó xác nhận symptom ban đầu tồn tại.

## Những gì cần quan sát

- Payload có đủ thông tin nghiệp vụ để gửi notification.
- Worker vẫn báo đã xử lý message.
- Không có deserialization exception.
- Một field cần thiết cho notification lại không có giá trị sau deserialize.
- So sánh assumptions của worker với behavior thường gặp trong ASP.NET Core JSON pipeline.

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
- message được đọc thành công
- `processed=1`
- `notified=0`

After:
- message vẫn được đọc thành công
- `processed=1`
- `notified=1`
- email lấy từ JSON input, không hard-code

## Estimated Time

30–45 phút.
