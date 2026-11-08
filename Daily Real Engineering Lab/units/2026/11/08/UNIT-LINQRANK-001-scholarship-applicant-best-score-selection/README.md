# UNIT-LINQRANK-001 — Hồ sơ học bổng: shortlist thay đổi theo thứ tự import

## Mục tiêu
Điều tra một sai lệch nghiệp vụ có thể reproduce, thiết kế regression test kiểm tra tính bất biến theo thứ tự input và giải thích quyết định chọn hồ sơ.

## Bối cảnh thực tế
Hệ thống xét học bổng nhận nhiều bản đánh giá cho cùng một ứng viên. Hội đồng chỉ muốn giữ bản có điểm cao nhất của mỗi ứng viên rồi chọn 2 ứng viên đứng đầu. Dữ liệu có thể đến từ các reviewer theo thứ tự khác nhau. Hai lần import cùng tập bản đánh giá đang tạo shortlist khác nhau.

## Bạn cần làm gì
Chạy fixture, ghi lại shortlist và mã revision; thay đổi thứ tự import; đề xuất ít nhất hai giả thuyết, sửa code trong `starter/Program.cs`, chứng minh kết quả bất biến với thứ tự input và quy tắc tie-break.

## Yêu cầu môi trường
.NET SDK 8.x; PowerShell 7+ trên Windows. Không cần database, cloud hoặc NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet run --project ./starter/ScholarshipRank.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy bản starter và yêu cầu chương trình trả mã lỗi khác 0 khi phát hiện lệch shortlist. Đọc hai bộ dữ liệu cùng nội dung nhưng khác thứ tự.

## Những gì cần quan sát
Ghi lại ứng viên nào được chọn, revision nào đại diện cho mỗi ứng viên và kết quả khi đảo thứ tự. Đừng chỉ so sánh tổng điểm; kiểm tra identity và tie-break.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
Trước: `reproduce.ps1` xác nhận lỗi và `verify.ps1` thất bại. Sau: shortlist chính xác, cùng kết quả cho mọi permutation kiểm thử, tie-break ổn định và `verify.ps1` thành công.

## Estimated Time
45 phút.
