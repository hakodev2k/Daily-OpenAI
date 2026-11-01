# UNIT-RWLOCK-001 — Bản ghi cảm biến mới khiến dịch vụ tra ngưỡng bị lỗi

## Mục tiêu
Điều tra sự khác biệt giữa luồng đọc dữ liệu đã có và luồng khởi tạo dữ liệu mới trong một registry dùng chung giữa nhiều request. Thu thập bằng chứng, đề xuất thay đổi tối thiểu và chứng minh tính đúng đắn khi có nhiều request đồng thời.

## Bối cảnh thực tế
Dịch vụ quản lý khí hậu của một bảo tàng lưu ngưỡng cảnh báo nhiệt độ theo mã cảm biến. Các mã đã có trong registry hoạt động bình thường. Sau khi lắp thêm cảm biến, yêu cầu đầu tiên cho mã mới trả lỗi; các lần đọc mã cũ vẫn thành công. Bộ phận vận hành cần đảm bảo không tạo trùng cấu hình khi nhiều thiết bị gửi tín hiệu cùng lúc.

## Bạn cần làm gì
1. Chạy starter và tái hiện sự cố ở nhánh mã cảm biến chưa có.
2. Ghi lại exception, call stack, khác biệt giữa cache hit và cache miss.
3. Đề xuất ít nhất hai giả thuyết; kiểm tra đường đi khi hai request cùng truy cập một mã.
4. Chỉnh sửa **chỉ** `starter/ThresholdRegistry.cs` để xử lý cả mã mới và mã đã có, giữ nguyên hành vi trả về.
5. Chạy verification trên chính starter đã sửa; so sánh với reference solution sau khi hoàn tất.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7+; .NET SDK 8 (xem `global.json`).
- Không cần database, Docker, NuGet package ngoài hay tài khoản cloud.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet --version
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Trước khi sửa code, chạy `./scripts/reproduce.ps1`. Script chỉ **PASS** khi starter tái hiện được lỗi dự kiến, không có nghĩa là lỗi đã được sửa. Đọc thông báo và ghi lại mã cảm biến nào kích hoạt lỗi.

## Những gì cần quan sát
- Kết quả tra cứu một mã đã tồn tại và một mã chưa có.
- Kiểu exception và stack trace trên đường đi tạo cấu hình.
- Sau khi sửa: tất cả 24 request đồng thời cho cùng mã nhận cùng giá trị; số lần tạo thực tế là một.
- Các request cho mã khác vẫn hoạt động.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Chạy `./scripts/verify.ps1` để verify correctness và concurrency invariant.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và code tham khảo](solution/README.md) — chỉ đọc sau khi tự điều tra và sửa.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
35–45 phút (L1 / D3).
