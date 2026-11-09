Gateway nhận 2 request với payload và idempotency key giống nhau; lần 1 trả 503, lần 2 trả 200; `verify.ps1` trả 0 khi learner code đúng.
