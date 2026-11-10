# UNIT-HASHMUT-001 — Ngân hàng hạt giống: lô hàng biến mất khỏi bộ chỉ mục sau chuyển kho

## Mục tiêu
Điều tra sự sai lệch giữa dữ liệu tồn kho và kết quả tra cứu trong một bộ chỉ mục in-memory; chứng minh nguyên nhân bằng một phép thử có thể lặp lại.

## Bối cảnh thực tế
Ngân hàng hạt giống cộng đồng quản lý các lô bảo quản theo mã và vị trí kệ. Sau khi chuyển lô sang kệ lạnh, màn hình tìm kiếm báo không tìm thấy, mặc dù báo cáo tổng số lô không thay đổi.

## Bạn cần làm gì
1. Chạy starter để ghi nhận số lô, kết quả tra cứu trước và sau chuyển kệ.
2. Ghi ít nhất hai hypothesis và bằng chứng loại trừ vào [workspace](workspace/investigation.md).
3. Sửa đường code learner-editable trong `starter/Program.cs` mà không làm mất tính năng chuyển kệ.
4. Chạy verification, đối chiếu [expected results](verification/cases.md).
5. Chỉ mở hints và solution sau khi tự điều tra.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7+ cho script; hoặc dùng trực tiếp `dotnet run`. Không cần database, Docker, Internet hoặc NuGet ngoài SDK.

## Chạy nhanh
```powershell
dotnet run --project starter/SeedBank.csproj -- reproduce
```

## Cách reproduce vấn đề
Chạy lệnh trên từ thư mục unit. Quan sát một lô đã tồn tại và được chuyển kệ. Sau đó chạy `./verify.ps1` hoặc `dotnet run --project starter/SeedBank.csproj -- verify`.

## Những gì cần quan sát
Ghi nhận `Count`, kết quả `Contains`, `Remove` và mã lô trước/sau thay đổi. Phân biệt dữ liệu còn tồn tại với khả năng tìm lại dữ liệu qua collection.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/01.md)
- [Hint 2](hints/02.md)
- [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Reference](solution/reference.md) — chỉ mở sau khi tự sửa.

## Expected Results
Trước sửa: lô vẫn được đếm nhưng thao tác tra cứu/xóa theo đối tượng thất bại sau chuyển kệ. Sau sửa: lô được tra cứu và xóa nhất quán; mã lô khác không bị nhận nhầm.

## Estimated Time
35 phút (L1 / D4).
