# UNIT-ES-001 — Search vừa ghi xong nhưng chưa thấy ngay

## Mục tiêu

Điều tra một tình huống read-after-write không nhất quán giữa hai kiểu đọc trên cùng dữ liệu search, thu thập evidence và chọn cách xử lý phù hợp với yêu cầu nghiệp vụ.

## Bối cảnh thực tế

Một internal product-catalog tool ghi document mới vào search cluster rồi ngay lập tức chạy truy vấn để xác nhận dữ liệu đã xuất hiện. Ở local và CI, thao tác ghi báo thành công, truy vấn theo ID có thể thấy document, nhưng truy vấn tìm kiếm ngay sau đó đôi khi trả về 0 kết quả. Nếu đợi thêm một khoảng ngắn thì kết quả xuất hiện.

Business yêu cầu luồng "publish rồi preview" phải nhìn thấy item vừa publish trước khi trả success cho editor. Đồng thời không được dùng một cách xử lý làm giảm throughput toàn hệ thống một cách không cần thiết.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi evidence vào `workspace/my-investigation.md`.
3. Đưa ra ít nhất 2 hypothesis trước khi xem hints.
4. Sửa code trong `starter/` để đáp ứng read-after-write requirement của workflow này.
5. Chạy `verify.ps1`.
6. Sau đó mới đối chiếu với reference solution.

## Yêu cầu môi trường

- Docker Desktop hoặc Docker Engine
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị
- Port `9200` đang rảnh

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` sẽ:

- khởi động Elasticsearch local bằng Docker
- tạo index dành riêng cho lab
- chạy starter
- kiểm tra output của luồng ghi rồi đọc ngay

Nếu mọi thứ đúng với starter ban đầu, bạn sẽ thấy thao tác ghi thành công nhưng hai kiểu đọc không cho cùng một trạng thái quan sát tại cùng thời điểm.

## Những gì cần quan sát

- HTTP status của write request
- Kết quả đọc trực tiếp theo document ID
- Số hit của search query ngay sau write
- Số hit của search query sau khi chờ
- Sự khác biệt giữa correctness requirement của workflow và throughput/latency cost

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã thử điều tra và fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:

- write request thành công
- direct lookup có thể thấy document mới
- immediate search có thể chưa thấy document
- delayed search thấy document

After:

- workflow publish/preview đáp ứng read-after-write requirement một cách có chủ đích
- `verify.ps1` pass
- không phụ thuộc vào sleep ngẫu nhiên

Nếu không reproduce được, chạy `docker compose down -v`, sau đó chạy lại `reproduce.ps1` để có index sạch.

## Estimated Time

35–55 phút
