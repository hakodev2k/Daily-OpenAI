# Incident evidence — synthetic

| Giờ | n_dead_tup | Tuổi transaction lâu nhất | p95 đối soát | CPU primary | autovacuum runs (cumulative) |
|---|---:|---:|---:|---:|---:|
| 00:00 | 120,000 | 2 phút | 75 ms | 39% | 114 |
| 06:00 | 890,000 | 6 giờ | 230 ms | 42% | 118 |
| 12:00 | 1,710,000 | 12 giờ | 510 ms | 45% | 123 |
| 18:00 | 2,540,000 | 18 giờ | 780 ms | 47% | 128 |

Log rút gọn:
- 00:02 export worker started report R-184; session txn isolation repeatable read
- 06:02 export worker heartbeat; no checkpoint completed
- 12:02 export worker heartbeat; no checkpoint completed
- 18:02 export worker heartbeat; no checkpoint completed
- 18:15 DBA sees VACUUM completed; dead tuple estimate remains high
- 18:22 replication lag = 3 seconds; disk free 31%; no sustained lock waits

Noise / competing clues:
- Deploy API lúc 11:00; request rate tăng 8% sau deploy
- Một query analytics không có index chạy 2 lần lúc 09:00

SQL read-only để kiểm tra trên môi trường giả định (không chạy trực tiếp lên production nếu chưa được cấp quyền):
```sql
SELECT pid, state, xact_start, backend_xmin, query
FROM pg_stat_activity WHERE xact_start IS NOT NULL ORDER BY xact_start;
SELECT relname, n_dead_tup, vacuum_count, autovacuum_count
FROM pg_stat_user_tables WHERE relname = 'parking_ledger';
SELECT datname, age(datfrozenxid) FROM pg_database;
```
Lưu ý: n_dead_tup là estimate; vacuum_count tăng không chứng minh đã reclaim được mọi tuple. Các dữ kiện tương quan chưa đủ để kết luận nguyên nhân duy nhất.
