# Sau khi sửa

Chạy `./scripts/verify.ps1` trên starter đã chỉnh: exit code 0. `BurstProviderCalls=1`, `WarmCacheHit=True`, `AllQuotesAgree=True`, `IndependentKeysOverlap=True`, `FailedFetchCanRetry=True`.

Không được làm các key khác nhau bị serialize, giữ dữ liệu cũ quá TTL hoặc giữ mãi Task lỗi. Tùy tốc độ máy, thời gian chạy có thể khác; không assert theo mili-giây.
