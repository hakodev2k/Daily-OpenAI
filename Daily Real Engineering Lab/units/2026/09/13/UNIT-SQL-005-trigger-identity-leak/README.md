# UNIT-SQL-005 — Insert thành công nhưng API trả sai ID sau trigger

## Bối cảnh

Warehouse Receiving API tạo một `Receipt` mới trong SQL Server rồi trả ID để client tiếp tục upload chứng từ. Sau khi team thêm audit trigger, insert vẫn thành công nhưng thỉnh thoảng client truy vấn ID vừa nhận lại thấy không đúng bản ghi vừa tạo.

Mục tiêu của lab là reproduce sai lệch này, thu thập bằng chứng, xác định boundary của identity value và sửa contract trả ID mà không vô hiệu hóa audit trigger.

## Yêu cầu môi trường

- Docker Desktop hoặc SQL Server local
- PowerShell 7+
- `sqlcmd`

Nếu dùng Docker, chạy `scripts/setup.ps1` để tạo SQL Server container và seed database.

## Chạy nhanh

```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy script starter và yêu cầu quan sát:

- row `Receipt` được tạo thành công
- audit row cũng được tạo
- giá trị ID mà starter trả về không trỏ đến `Receipt` vừa insert

## Những gì cần quan sát

Ghi lại ít nhất ba bằng chứng:

1. ID thực tế của row trong `dbo.Receipts`.
2. ID của row audit sinh ra bởi trigger.
3. ID mà starter trả về cho caller.

Không xem solution trước khi có hypothesis.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Sửa `starter/create-receipt.sql`.
4. Chạy `./scripts/verify.ps1`.
5. Sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- business row và audit row đều được insert.
- script trả về một identity value không đại diện cho business row vừa tạo.

After:
- insert vẫn kích hoạt audit trigger.
- returned ID luôn đúng với `dbo.Receipts.Id` của row vừa tạo.
- verify script kết thúc với exit code `0`.

## Estimated Time

Khoảng 40 phút.
