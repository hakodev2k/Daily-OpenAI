# UNIT-STREAMPOS-001 — Bản ghi âm được xác thực nhưng lưu trữ rỗng

## Mục tiêu
Điều tra một lỗi pipeline xử lý stream bằng chứng checksum và byte integrity; sửa và kiểm chứng bằng regression.

## Bối cảnh thực tế
Dịch vụ lưu bản ghi âm động vật hoang dã báo checksum hợp lệ nhưng bản sao archive đôi khi không chứa dữ liệu. Bạn cần xác định điểm dữ liệu bị mất trong hai công đoạn xử lý.

## Bạn cần làm gì
Reproduce trên starter; ghi hypothesis, evidence; chỉnh `starter/Program.cs` để thỏa contract lưu trữ.

## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần mạng, database hay package ngoài.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` thành công khi lỗi ban đầu xuất hiện; `verify.ps1` phải thất bại trước khi sửa và thành công sau khi sửa.

## Những gì cần quan sát
Đối chiếu `CHECKSUM_OK`, `STORED_BYTES`, `PAYLOAD_OK`. Đừng chỉ dựa vào checksum để khẳng định archive đúng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md)

## Expected Results
Trước sửa: checksum hợp lệ nhưng dữ liệu lưu không khớp. Sau sửa: checksum hợp lệ, toàn bộ bytes được lưu, verify exit 0.

## Estimated Time
35–45 phút.
