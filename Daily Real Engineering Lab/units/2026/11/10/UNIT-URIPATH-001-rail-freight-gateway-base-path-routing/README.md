# UNIT-URIPATH-001 — Tuyến API của đối tác vận tải đường sắt bị chuyển sai gateway

## Mục tiêu
Điều tra cách URI outbound được tạo khi gateway có prefix theo đối tác; phân biệt lỗi routing với lỗi xác thực và DNS bằng evidence thực thi.

## Bối cảnh thực tế
Một hệ thống điều phối container gửi request đến gateway chung. Sau khi đối tác chuyển endpoint sang namespace `/partners/v2/`, health check của gateway vẫn xanh nhưng các request nghiệp vụ trả 404. Không có thay đổi DNS hoặc credentials.

## Bạn cần làm gì
1. Chạy starter và ghi lại URI thực tế của ba request.
2. Đưa ra ít nhất hai giả thuyết và một cách loại trừ mỗi giả thuyết.
3. Chỉnh code tại `starter/Program.cs` để ba route giữ đúng namespace, không thay đổi host hay query.
4. Chạy verification và giải thích vì sao lỗi chỉ xuất hiện ở gateway có path prefix.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell; không cần network hay secret.

## Chạy nhanh
```powershell
dotnet run --project starter/RailGateway.csproj -- --reproduce
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script kiểm tra cả ba trường hợp ban đầu đều sai URI nhưng kết thúc thành công khi tái hiện đúng symptom.

## Những gì cần quan sát
So sánh `expected` và `actual` của từng case; lưu evidence trong ghi chú cá nhân. Tìm sự khác biệt giữa host, path và query. Không sửa expected values để đạt PASS.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Trước sửa: ba case hiển thị FAIL và reproduce thành công. Sau sửa: ba case PASS; `./verify.ps1` exit 0. Kiểm tra thêm route bắt đầu bằng slash và base thiếu slash theo hợp đồng bạn chọn.

## Estimated Time
35–45 phút.
