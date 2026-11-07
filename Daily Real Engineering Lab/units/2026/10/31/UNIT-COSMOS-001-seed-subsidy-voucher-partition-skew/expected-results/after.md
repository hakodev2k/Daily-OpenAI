# After — Learner policy

Chạy `pwsh -NoProfile -File ./verify.ps1`: yêu cầu `PASS`, exit code 0, `maxBucketShare <= 0.22`, `replayStable=True`, `pointReadFromVoucherId=True`. Độ lệch bucket cụ thể có thể khác khi thay đổi fixture; không dùng chỉ số synthetic làm cam kết RU/s hoặc p95 trên Cosmos DB thực. Thực nghiệm cloud cần metrics normalized RU, 429 theo partition, query RU, latency và rollback rehearsal.
