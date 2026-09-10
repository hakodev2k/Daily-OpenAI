# UNIT-API-002 — Idempotency Key Payload Mismatch

## Mục tiêu

Điều tra một payment API có cơ chế idempotency nhưng trả về kết quả của request trước cho một request nghiệp vụ khác khi client tái sử dụng cùng key. Mục tiêu là xác định contract idempotency đúng và sửa mà vẫn ngăn duplicate side effect.

## Bối cảnh thực tế

Mobile client gửi lệnh charge với `Idempotency-Key`. Retry cùng request phải an toàn. Tuy nhiên telemetry cho thấy một request mới có payload khác đôi khi nhận receipt thuộc order trước đó, trong khi payment provider không nhận charge mới.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại evidence và ít nhất 2 hypothesis.
3. Xác định invariant mà idempotency layer đang thiếu.
4. Sửa code trực tiếp trong `starter/`.
5. Chạy `verify.ps1` để chứng minh retry cùng payload vẫn an toàn, còn reuse key với payload khác không được trả nhầm receipt.
6. Sau đó mới xem reference solution.

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

Script chạy starter với hai charge request khác nhau nhưng cùng một idempotency key và kiểm tra symptom quan sát được.

## Những gì cần quan sát

- Request đầu tiên tạo đúng một provider call.
- Request sau dùng cùng key nhưng payload khác.
- Provider call count không tăng.
- Response của request sau không đại diện đúng cho payload vừa gửi.

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
- first request succeeds
- second request reusing the key with different payload receives an incorrect prior result
- provider is called once

After:
- retry with the same key and same payload returns the original result without another provider call
- same key with different payload is rejected explicitly
- provider remains protected from duplicate side effects

Nếu không reproduce được, chạy `dotnet run --project starter/IdempotencyLab.csproj` và kiểm tra các dòng `first`, `retry`, `mismatch`, `providerCalls`.

## Estimated Time

30–45 phút.
