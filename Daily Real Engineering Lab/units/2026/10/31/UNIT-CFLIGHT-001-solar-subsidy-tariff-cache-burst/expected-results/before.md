# Trước khi sửa

Chạy `./scripts/reproduce.ps1` trên starter chưa chỉnh: exit code 0 và in `WarmCacheHit=True`, `AllQuotesAgree=True`, `IndependentKeysOverlap=True`, `FailedFetchCanRetry=True`; `BurstProviderCalls` thường là 12 và **lớn hơn 1**. `./scripts/verify.ps1` phải trả exit code 1.

Nếu không thấy triệu chứng: kiểm tra .NET SDK, không sửa contract; chạy lại reproduce và ghi output. Burst dùng 12 Task có gate khởi động chung và fake upstream 200ms; không dựa vào dịch vụ mạng.
