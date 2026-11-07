# UNIT-SQL-009 — Đối soát xác nhận đối tác trả về danh sách trống

## Mục tiêu
Điều tra một sai lệch dữ liệu đối soát bằng SQL có khả năng tái hiện ổn định; dùng dữ liệu thực nghiệm để phân biệt lỗi ingest, lọc tenant, batch và ngữ nghĩa truy vấn. Xác nhận bản sửa bằng assertion độc lập với câu SQL do bạn viết.

## Bối cảnh thực tế
Dịch vụ bảo hiểm nhận danh sách hợp đồng đã được đối tác xác nhận theo batch. Mỗi sáng, job cần liệt kê hợp đồng **ACTIVE** của tenant A chưa có xác nhận trong batch 42 để chuyển cho nhóm vận hành. Dashboard gần đây trả về danh sách trống dù bộ phận nghiệp vụ đã chỉ ra hai hợp đồng chưa được xác nhận.

## Bạn cần làm gì
1. Chạy baseline, ghi lại số dòng và bằng chứng dữ liệu đầu vào.
2. Đưa ra ít nhất ba hypothesis (ingest, scope, SQL semantics), rồi kiểm tra từng hypothesis bằng câu lệnh SQL của riêng bạn.
3. Sửa **starter/query.sql**; giữ nguyên bảng dữ liệu, yêu cầu tenant, batch và ACTIVE.
4. Chạy verify để chứng minh tập kết quả chính xác và không lẫn tenant/batch.

## Yêu cầu môi trường
Docker Desktop / Docker Engine với Docker Compose v2; PowerShell 7+ khuyến nghị. Không cần tài khoản cloud hoặc dịch vụ trả phí. Container PostgreSQL 16.4 chỉ chạy nội bộ Docker network, không publish port.

## Chạy nhanh
Tại thư mục unit:

```powershell
./scripts/reproduce.ps1
# Sau khi sửa starter/query.sql:
./scripts/verify.ps1
```

Nếu PowerShell chặn script: dùng `pwsh -NoProfile -File ./scripts/reproduce.ps1` và `pwsh -NoProfile -File ./scripts/verify.ps1`.

## Cách reproduce vấn đề
`reproduce.ps1` reset seed, chạy **baseline cố định** ở `scripts/original-query.sql`, rồi xác nhận triệu chứng. Baseline này không bị thay đổi khi bạn sửa starter. Muốn xem kết quả thủ công:

```powershell
docker compose -p unit-sql-009 exec -T db psql -X -U lab -d lab -c "TABLE lab.missing_enrollments;"
docker compose -p unit-sql-009 exec -T db psql -X -U lab -d lab -c "TABLE lab.partner_confirmations;"
```

## Những gì cần quan sát
- Số hợp đồng ACTIVE của tenant A, các xác nhận thuộc batch 42 và các bản ghi từ batch/tenant khác.
- Kết quả đối soát hiện tại có phù hợp kỳ vọng nghiệp vụ không?
- Có dòng đầu vào nào không có mã hợp đồng hợp lệ? Ghi lại trước khi sửa.
- Thử các truy vấn chẩn đoán không ghi dữ liệu để khoanh vùng nguyên nhân.

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
**Spoiler — chỉ mở sau khi tự sửa:** [Giải pháp tham chiếu](solution/README.md) · [SQL tham chiếu](solution/query.sql)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
45–60 phút (L2 Investigation, D4). Ghi nhận điều tra tại [workspace](workspace/my-investigation.md).
