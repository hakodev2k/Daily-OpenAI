# UNIT-PGIDX-001 — Import thẻ thư viện: UPSERT lỗi dù unique index vẫn tồn tại

## Mục tiêu
Điều tra contract giữa câu lệnh import và ràng buộc uniqueness của PostgreSQL; tái hiện lỗi, đọc catalog index, đề xuất thay đổi nhỏ và verify trên dữ liệu soft-delete.

## Bối cảnh thực tế
Hệ thống thư viện liên vùng nhập thẻ từ đối tác theo `branch_id` và `external_card`. Khi một thẻ được soft-delete, lịch sử phải được giữ lại, nhưng thẻ đang hoạt động phải duy nhất trong mỗi chi nhánh. Sau migration policy, import bắt đầu thất bại. Đội vận hành thấy index vẫn tồn tại và không có duplicate active row.

## Bạn cần làm gì
1. Chạy setup và reproduce, lưu nguyên văn PostgreSQL diagnostic.
2. Chạy `./run.ps1` để xem định nghĩa index và dữ liệu fixture.
3. Viết hai hypothesis trong [workspace](workspace/my-investigation.md) trước khi đọc hints.
4. Chỉ sửa `starter/upsert.sql`, giữ nguyên schema và harness.
5. Chạy verify để kiểm tra cả lần import đầu và lần import lặp lại, tính độc lập chi nhánh và lịch sử đã xóa.

## Yêu cầu môi trường
Docker Engine + Docker Compose v2 và PowerShell 7+. Image `postgres:16.4` được pin; không cần tài khoản cloud hay NuGet. Cổng local 55432.

## Chạy nhanh
~~~powershell
./setup.ps1
./reproduce.ps1
./run.ps1
~~~
Sau khi sửa starter: `./verify.ps1`. Linux/macOS có thể dùng lệnh tương đương `docker compose -f compose.yaml exec -T postgres psql -X -v ON_ERROR_STOP=1 -U lab -d lab -f /lab/harness/verify.sql` sau `docker compose up -d --wait` và chạy schema.

## Cách reproduce vấn đề
`reproduce.ps1` kiểm tra diagnostic dự kiến trên starter gốc. Exit 0 nghĩa là **đã reproduce được lỗi**, không phải đã sửa. `verify.ps1` đọc trực tiếp `starter/upsert.sql` từ container và trả exit 0 chỉ khi mọi assertion đạt.

## Những gì cần quan sát
- Index tồn tại nhưng lệnh import vẫn bị PostgreSQL từ chối.
- Bản ghi soft-delete và active cùng business key có thể cùng tồn tại theo policy.
- Import lặp lại phải cập nhật đúng active row mà không làm thay đổi lịch sử hay chi nhánh khác.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút.
