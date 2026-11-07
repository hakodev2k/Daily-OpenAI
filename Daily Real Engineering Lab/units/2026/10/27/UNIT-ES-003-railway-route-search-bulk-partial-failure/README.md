# UNIT-ES-003 — Railway Search: HTTP 200 nhưng thiếu tuyến trong index

## Mục tiêu
Điều tra một sự cố đồng bộ dữ liệu tìm kiếm; phân biệt kết quả của HTTP request với kết quả từng thao tác trong một bulk operation, thu thập bằng chứng và xác minh không mất bản ghi âm thầm.

## Bối cảnh thực tế
Dịch vụ đồng bộ lịch tàu đêm đẩy bốn tuyến vào search index. Dashboard báo request thành công, nhưng nhân viên điều độ không tìm thấy hai tuyến. API upstream không báo lỗi HTTP. Tập dữ liệu nhỏ này mô phỏng một lần import; mọi dữ liệu đều synthetic.

## Bạn cần làm gì
1. Chạy starter, lưu lại kết quả import được báo cáo và trạng thái index thực tế.
2. Đọc payload phản hồi của fake search gateway và phân loại từng tuyến.
3. Viết ít nhất hai giả thuyết; đối chiếu chúng với bằng chứng trước khi sửa.
4. Chỉ sửa `starter/BulkIngestor.cs`: giữ tính toàn vẹn dữ liệu, xử lý lỗi có thể khôi phục và lỗi cần cách ly; tránh gửi lại những mục đã thành công.
5. Chạy verify, ghi quyết định retry và ranh giới production vào workspace.

## Yêu cầu môi trường
.NET SDK 8 trở lên có khả năng build target `net8.0`; PowerShell 7 hoặc Windows PowerShell 5.1. Không cần Elasticsearch, Docker, NuGet package ngoài hay cloud subscription. Đây là mô phỏng **bulk response contract**, không phải một Elasticsearch server hoàn chỉnh.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./reproduce.ps1
# Sửa starter/BulkIngestor.cs
./verify.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy **starter gốc**, kiểm tra marker `CHECK_FAILED` và chỉ thành công khi phát hiện đúng triệu chứng. Không dùng kết quả `reproduce.ps1` để kết luận fix đã đúng.

## Những gì cần quan sát
- `HTTP_STATUS` của gateway và `INDEXED_REPORTED` do client công bố có khớp `INDEXED_ACTUAL`?
- Những document ID nào được gửi bao nhiêu lần?
- Dữ liệu nào phải được lưu để có thể xử lý sau khi job kết thúc?
- Log `CHECK_FAILED` là kết quả kiểm tra business invariant, không phải lỗi build.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [solution/README.md](solution/README.md) và [reference code](solution/BulkIngestor.cs) sau khi tự thử.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
60–75 phút. Ghi nhận investigation tại [workspace/my-investigation.md](workspace/my-investigation.md).
