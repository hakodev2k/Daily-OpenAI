# Incident Evidence

Procedure: `dbo.SearchShipments @TenantId`

## Data distribution
- Tenant A: khoảng 42 rows.
- Tenant B: khoảng 1,850,000 rows.
- Các tenant còn lại: phần lớn 100–4,000 rows.

## Snapshot 1 — 09:10
- Service restart lúc 09:02.
- Execution đầu tiên sau restart dùng Tenant A.
- Tenant A: 18 ms, 180 logical reads.
- Tenant B sau đó: 4.8 s, khoảng 1.9M logical reads.
- Actual rows tại operator chính của Tenant B lớn hơn estimate nhiều bậc độ lớn.

## Snapshot 2 — 13:40
- Procedure được recompiled trong quá trình troubleshooting.
- Execution đầu tiên sau compile dùng Tenant B.
- Tenant B: 620 ms, 31k logical reads.
- Tenant A sau đó: 95 ms, 8.5k logical reads.

## Noise / thông tin phụ
- App CPU 32–38% ở cả hai giai đoạn.
- SQL Server memory pressure alert: không có.
- Network RTT app → DB: 2–4 ms.
- Deployment không đổi schema hoặc application code.

## Câu hỏi điều tra
1. Evidence nào giải thích tính phụ thuộc vào execution history?
2. Evidence nào chỉ là noise?
3. Bạn cần lấy thêm artifact nào từ SQL Server để xác nhận hypothesis?
4. Mitigation nào phù hợp nếu workload thực sự có hai nhóm cardinality rất khác nhau?