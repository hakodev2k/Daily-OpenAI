# UNIT-MEMCAP-001 — Confidential export trả về payload dài bất thường

## Mục tiêu
Điều tra lỗi byte-integrity khi một service .NET tạo báo cáo nhạy cảm trong bộ nhớ rồi gửi qua HTTP.

## Bối cảnh thực tế
Một hệ thống kiểm toán nội bộ xuất bản ghi đã được lọc. Dữ liệu gốc chỉ dài vài byte, nhưng downstream nhận nội dung dài hơn, checksum không khớp và một số client từ chối file.

## Bạn cần làm gì
Chạy starter, ghi lại độ dài logic, số byte được xuất, checksum và kiểm tra xem payload thực tế có đúng hợp đồng hay không. Nêu tối thiểu hai giả thuyết trước khi sửa.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7 hoặc Windows PowerShell; không cần Docker, database hay cloud.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`, sau đó `dotnet run --project starter/ExportProbe.csproj -- --check`. Lệnh thứ hai dự kiến trả exit code 2 khi còn lỗi.

## Những gì cần quan sát
So sánh `LogicalLength` và `ExportedLength`, số byte của payload mong đợi, và kết quả `--check`. Đừng suy luận từ console text đơn thuần vì ký tự NUL có thể khó nhìn thấy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
Trước: 8 byte logic nhưng số byte xuất lớn hơn và `--check` thất bại. Sau: xuất đúng 8 byte, hash tính trên chính 8 byte, `--check` thành công. Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
30–40 phút.
