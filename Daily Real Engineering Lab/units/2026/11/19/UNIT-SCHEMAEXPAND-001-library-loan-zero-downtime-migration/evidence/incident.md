# Evidence pack — dữ liệu mô phỏng
## Hệ thống
SQL Server: Loan (48 triệu rows), 700 writes/s lúc cao điểm, 4 ASP.NET Core instances. Old V1 ghi BorrowerCode nullable, new V2 muốn BorrowerKey NOT NULL và unique. Deploy rolling 30 phút. SLO p95 < 250 ms, error < 0.5%. Không cho downtime.
## Migration M0 (đề xuất ban đầu)
```sql
BEGIN TRAN;
ALTER TABLE dbo.Loan ADD BorrowerKey nvarchar(80) NOT NULL DEFAULT N'UNKNOWN';
CREATE UNIQUE INDEX UX_Loan_BorrowerKey ON dbo.Loan(BorrowerKey);
COMMIT;
```
## Timeline staging
| Giây | Dấu hiệu | Số đo |
|---|---|---|
| 0 | bắt đầu deploy V2 | p95 105 ms |
| 12 | migration chạy | waiting LCK_M_SCH_M 9 s |
| 18 | nhiều API requests chờ | p95 4.8 s, error 7.2% |
| 24 | migration thất bại | SQL error 1505 duplicate key 'UNKNOWN' |
| 27 | V2 instance đầu tiên nhận traffic | lỗi Invalid column name BorrowerKey |
| 35 | rollback V2 về V1 | V1 ổn định sau 60 s |
## Additional signals
CPU SQL 36–42%, buffer cache hit 98%; log disk 35% utilization. Truy vấn SELECT Loan WHERE LoanId=@id thường 8 ms. Trong staging, một transaction reporting giữ schema stability reference lâu 22 s. Các giá trị BorrowerCode không duy nhất: nhiều khoản vay có thể cùng một người.
## Constraints
Không được đổi semantics: một BorrowerKey định danh người mượn, không phải khoản vay. Backfill phải resumable; không được ghi đè cập nhật mới hơn. Ứng dụng V1/V2 cùng tồn tại, có thể rollback V2 sau khi đã có dữ liệu mới. Quyền chạy DDL chỉ có trong deployment window.
