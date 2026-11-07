# UNIT-PG-001 — Transaction Aborted After Error

## Mục tiêu

Điều tra một batch import dùng PostgreSQL transaction: một record trùng được ứng dụng bắt exception và bỏ qua, nhưng record hợp lệ ngay sau đó vẫn thất bại. Bạn cần dùng output và trạng thái dữ liệu để xác định contract thực tế của transaction, sau đó sửa batch sao cho giữ được yêu cầu nghiệp vụ.

## Bối cảnh thực tế

Một worker đồng bộ customer từ hệ thống đối tác nhận một batch có thể chứa `external_id` đã tồn tại. Yêu cầu nghiệp vụ là bỏ qua duplicate nhưng vẫn ghi các customer hợp lệ còn lại trong **cùng một batch transaction**. Production log cho thấy duplicate đã được catch, tuy nhiên các insert kế tiếp vẫn không chạy thành công.

## Bạn cần làm gì

1. Khởi động PostgreSQL local.
2. Chạy starter và reproduce symptom.
3. Ghi lại evidence và ít nhất hai hypothesis trong `workspace/my-investigation.md`.
4. Sửa trực tiếp code trong `starter/` để duplicate không làm hỏng phần còn lại của batch.
5. Giữ nguyên yêu cầu: duplicate được bỏ qua, customer hợp lệ được lưu, batch commit thành công.
6. Chạy `verify.ps1` trên code bạn đã sửa.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- Docker Desktop hoặc Docker Engine có Docker Compose
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./setup.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script reset database state rồi chạy starter. Reproduction thành công khi output chứng minh rằng lỗi duplicate đã được catch nhưng một command hợp lệ sau đó vẫn gặp transaction-state failure.

## Những gì cần quan sát

- Record đầu tiên được insert thành công.
- Record thứ hai có cùng business key và phát sinh lỗi được ứng dụng xử lý.
- Record thứ ba có business key khác nhưng vẫn không được insert.
- Output có SQLSTATE của các failure.
- Sau batch, dữ liệu không phản ánh kết quả nghiệp vụ mong muốn.

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

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- duplicate được catch ở application layer
- command hợp lệ sau duplicate vẫn thất bại
- `ABORTED_STATE_OBSERVED=true`
- batch không commit kết quả mong muốn

After:
- duplicate được bỏ qua theo policy
- customer hợp lệ sau duplicate vẫn được insert
- `BATCH_COMMITTED=true`
- `ROW_COUNT=2`
- `verify.ps1` pass

## Estimated Time

35–55 phút.
