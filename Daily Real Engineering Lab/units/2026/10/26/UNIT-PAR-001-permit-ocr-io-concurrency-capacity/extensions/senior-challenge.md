# Senior extension
Đối tác đổi quota thành 40 RPS, tối đa 8 in-flight; p99 tăng lên 2 s và 5% request trả 429. Thiết kế policy tách request rate khỏi concurrency, retry có jitter và idempotency. Đề xuất metric, overload behavior, test partial failure, rollback; không mặc định tăng replica.
