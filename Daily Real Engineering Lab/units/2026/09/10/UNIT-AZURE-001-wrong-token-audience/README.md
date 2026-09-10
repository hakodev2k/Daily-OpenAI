# UNIT-AZURE-001 — Token lấy được nhưng Azure resource vẫn trả 401

## Mục tiêu

Điều tra một lỗi authentication trong đó ứng dụng lấy access token thành công nhưng downstream resource vẫn từ chối request.

## Bối cảnh thực tế

Một background worker dùng Managed Identity để gọi Azure Blob Storage. Sau một thay đổi cấu hình, bước lấy token vẫn thành công và không có exception từ credential, nhưng mọi request tới resource đều trả `401 Unauthorized`.

## Bạn cần làm gì

1. Reproduce lỗi bằng starter app.
2. Ghi lại evidence trước khi sửa.
3. Xác định vì sao token hợp lệ về mặt phát hành nhưng không hợp lệ với resource đích.
4. Sửa `starter/`.
5. Chạy `verify.ps1` để chứng minh request thành công mà không thay đổi resource-side authorization rule.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Azure subscription; lab dùng local deterministic simulation.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Credential báo đã cấp token thành công.
- Token có metadata mô tả resource mà nó hướng tới.
- Fake resource server từ chối request với `401`.
- Việc cấp token thành công không đồng nghĩa token có thể dùng cho mọi Azure resource.

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

Trước fix:
- token được tạo thành công
- downstream trả `401`

Sau fix:
- token vẫn được tạo thành công
- downstream trả `200`
- verification xác nhận token được phát hành cho đúng resource boundary

Nếu không chạy được, kiểm tra `dotnet --info` và bảo đảm .NET 8 SDK có sẵn.

## Estimated Time

30–45 phút.
