# UNIT-AZURE-004 — Secret đã rotate nhưng ứng dụng vẫn dùng credential cũ

## Mục tiêu

Điều tra một lỗi cấu hình secret rotation khiến ứng dụng vẫn hoạt động bình thường trước khi rotate nhưng bắt đầu nhận `401 Unauthorized` sau khi credential phía downstream được thay đổi.

## Bối cảnh thực tế

Một service thanh toán đọc API credential từ Azure Key Vault. Team Security đã rotate secret và xác nhận version mới đang active. Tuy nhiên service vẫn gửi credential cũ và mọi request mới tới payment provider bắt đầu thất bại.

Trong lab này Azure Key Vault được mô phỏng local để tập trung vào contract version/refresh, không cần Azure subscription.

## Bạn cần làm gì

1. Chạy starter và reproduce lỗi.
2. Ghi lại evidence trước khi sửa.
3. Đưa ra ít nhất 3 hypotheses hợp lý.
4. Xác định vì sao service không quan sát credential mới.
5. Sửa `starter/` để service sử dụng secret hiện hành theo một refresh boundary rõ ràng.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script phải xác nhận starter tái hiện được tình trạng request bị từ chối sau secret rotation.

## Những gì cần quan sát

Thu thập tối thiểu:

- secret reference mà application đang sử dụng;
- version thực tế được resolver trả về;
- version hiện hành trong simulated vault;
- HTTP-like result từ simulated payment provider;
- sự khác biệt giữa trạng thái vault và credential application gửi đi.

Không sửa code trước khi bạn có ít nhất ba hypotheses.

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

> Spoiler: chỉ mở sau khi đã reproduce và tự thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- vault có credential version mới;
- application vẫn resolve một version cũ;
- downstream trả về trạng thái tương đương `401 Unauthorized`;
- `reproduce.ps1` coi đây là reproduction thành công.

### After

- application resolve credential hiện hành tại refresh boundary;
- downstream chấp nhận request;
- `verify.ps1` kết thúc với exit code `0`.

Nếu không reproduce được, kiểm tra `dotnet --version` và chạy trực tiếp:

```powershell
dotnet run --project ./starter/SecretRotationLab.csproj
```

## Estimated Time

Khoảng 45 phút.
