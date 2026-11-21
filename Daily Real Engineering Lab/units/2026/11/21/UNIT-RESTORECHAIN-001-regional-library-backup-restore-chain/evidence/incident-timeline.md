# Timeline — UTC+7, fixture giả lập

| Time | Evidence |
|---|---|
| 10:05 | L109 hoàn tất, media được kiểm tra checksum |
| 10:15 | L110 job SUCCESS nhưng restore-media probe trả I/O error |
| 10:17 | Volume database mất mount, API trả 503 |
| 10:22 | SEV-1, các writer khác bị đóng |
| 10:26 | Replica snapshot có nhưng chưa restore test |
| 10:31 | DBA bắt đầu restore trên standby |

RTO tính từ 10:17. Restore durations: full 19 phút; diff 8 phút; mỗi log 3 phút; integrity check 12 phút; application smoke 8 phút; cutover 5 phút. Các thời gian là ước lượng, không phải SLA. Tail-log availability chưa xác minh. LSN được đơn giản hóa cho bài tập; trong SQL Server thực cần kiểm tra backupset, recovery fork, differential base và chạy RESTORE thật. Không suy ra chính xác thời điểm transaction từ LSN.
