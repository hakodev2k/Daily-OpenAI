# UNIT-REGEXTIME-001 — Form đăng ký đô thị treo khi kiểm tra mã tham chiếu

## Mục tiêu
Điều tra một request xử lý lâu bất thường, thu thập thời gian chạy, đề xuất bản vá có giới hạn tài nguyên và kiểm thử regression.

## Bối cảnh thực tế
Cổng tiếp nhận phản ánh đô thị nhận mã tham chiếu do người dân nhập. Phần lớn mã hợp lệ được kiểm tra tức thì; một chuỗi gần giống mã hợp lệ khiến worker chậm nghiêm trọng. Nhóm vận hành cần ngăn một input làm suy giảm toàn bộ dịch vụ.

## Bạn cần làm gì
Chạy starter, ghi lại thời gian cho input bình thường và input bất thường, nêu ít nhất hai giả thuyết, sửa mã trong starter và chạy verify. Không mở solution trước khi điều tra.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7 hoặc Windows PowerShell; không cần dịch vụ ngoài.

## Chạy nhanh
```powershell
dotnet run --project starter/FormValidator.csproj -- normal
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy workload giới hạn để đo hai loại input và ghi bằng chứng. Nếu máy quá chậm, dừng bằng Ctrl+C. Không chạy benchmark với input dài hơn dữ liệu mẫu khi chưa kiểm soát timeout.

## Những gì cần quan sát
So sánh elapsed milliseconds, trạng thái hợp lệ và sự khác biệt khi thêm một ký tự cuối. Ghi lại phiên bản .NET và CPU nếu kết quả không tái hiện rõ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
Input hợp lệ vẫn được chấp nhận; input bất thường bị từ chối trong giới hạn thời gian; không gây exception chưa xử lý; test xác nhận đường đi starter đã sửa.

## Estimated Time
35–45 phút.
