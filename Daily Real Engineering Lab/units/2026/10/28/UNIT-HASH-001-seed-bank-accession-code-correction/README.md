# UNIT-HASH-001 — Mã giống cây đã sửa nhưng registry không tìm thấy

## Mục tiêu
Reproduce một lỗi dữ liệu âm thầm trong registry C#, thu thập bằng chứng trước/sau khi sửa mã, đưa ra giả thuyết và bảo đảm các thao tác tìm, sửa, xóa, chống trùng vẫn đúng.

## Bối cảnh thực tế
Ngân hàng hạt giống cộng đồng nhập các accession vào hệ thống để quản lý nguồn gốc. Nhân viên có thể sửa mã accession do nhập nhầm. Sau khi sửa từ `SEED-041` sang `SEED-042`, màn hình chi tiết vẫn hiển thị mã mới nhưng API tra cứu lại trả về không tìm thấy. Một lần nhập khác với cùng mã có thể được chấp nhận.

## Bạn cần làm gì
1. Chạy baseline, ghi lại kết quả lookup, duplicate, remove và count.
2. Đề xuất ít nhất hai giả thuyết dựa trên code và dữ liệu quan sát.
3. Chỉ sửa `starter/AccessionRegistry.cs` để giữ đúng hợp đồng khi đổi mã; không sửa fixture hoặc bộ kiểm thử.
4. Chạy `verify.ps1` trên code của bạn; thử thêm một test case.
5. Chỉ sau đó xem reference solution.

## Yêu cầu môi trường
.NET SDK 8 hoặc mới hơn có thể build `net8.0`; PowerShell 7 hoặc Windows PowerShell 5.1. Không cần database, Docker, tài khoản cloud hoặc NuGet package bên ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Dòng `REPRODUCTION_CONFIRMED` chỉ xác nhận baseline gốc tái hiện lỗi, **không** chứng minh code đã sửa đúng.

## Những gì cần quan sát
- Mã accession trên object trước/sau correction.
- Lookup theo mã mới có tìm thấy object hay không.
- Có thể đăng ký thêm accession với mã mới hay không.
- Xóa mã mới có làm số lượng còn lại về 0 hay không.
- Sự khác nhau giữa dữ liệu đang hiển thị và các thao tác tra cứu.

Bằng chứng mô phỏng: [incident](evidence/incident.md). Ghi chép: [my-investigation](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi tự điều tra:** [solution/README.md](solution/README.md) · [reference code](solution/AccessionRegistry.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
35 phút · L1 / D3 · Validation: static-review (chưa thực thi .NET build/tests trong lượt tạo).
