# UNIT-JSONEVOLVE-001 — Nâng cấp JSON contract cho dữ liệu tình trạng nhà trú ẩn

## Mục tiêu
Điều tra sự cố import JSON khi đối tác bổ sung giá trị trạng thái mới; cải tiến ranh giới tích hợp để dữ liệu hợp lệ không bị mất và giá trị chưa biết vẫn được ghi nhận có kiểm soát.

## Bối cảnh thực tế
Dịch vụ điều phối nơi trú ẩn nhận một batch trạng thái từ các điểm tiếp nhận. Sau khi đối tác triển khai phiên bản mới, batch có 3 bản ghi chỉ nhập được 0. Điều phối viên không nhìn thấy tình trạng của các địa điểm vẫn dùng trạng thái cũ.

## Bạn cần làm gì
1. Chạy ứng dụng starter, lưu stdout và mã thoát.
2. Đưa ra ít nhất hai giả thuyết, phân biệt lỗi payload với lỗi contract.
3. Chỉnh sửa **starter/Program.cs** để 3 bản ghi được xử lý, trạng thái chưa hỗ trợ không bị giả định là Available.
4. Giữ nguyên raw status và báo cáo số bản ghi cần xem xét.
5. Chạy verify trên chính starter đã sửa; chỉ mở solution sau khi ghi kết luận.

## Yêu cầu môi trường
.NET SDK 8.0 (không cần database, cloud hay NuGet ngoài framework).

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` phải xác nhận starter hiện tại không hoàn thành import. Script này kiểm tra **triệu chứng gốc**, không dùng để đánh giá bản sửa.

## Những gì cần quan sát
Đối chiếu số dòng JSON đầu vào với số bản ghi thực sự được nhập; quan sát liệu bản ghi hợp lệ trước và sau giá trị mới có bị ảnh hưởng không. Ghi lại raw payload của trạng thái lạ mà không sửa payload của đối tác.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Trước sửa: import thất bại, không in `IMPORTED=3`. Sau sửa: `IMPORTED=3`, `REVIEW=1`, `SHELTER=S1;STATUS=Available`, `SHELTER=S2;STATUS=Unknown`, `SHELTER=S3;STATUS=Full`, `RAW=S2:Standby`. Không được đánh dấu bản ghi chưa hiểu là Available.

## Estimated Time
45–55 phút.
