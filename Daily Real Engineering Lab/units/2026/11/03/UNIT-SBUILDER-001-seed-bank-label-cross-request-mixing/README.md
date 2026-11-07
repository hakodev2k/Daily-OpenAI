# UNIT-SBUILDER-001 — Nhãn vận chuyển ngân hàng hạt giống bị trộn giữa hai yêu cầu

## Mục tiêu

Điều tra một sai lệch dữ liệu chỉ xuất hiện khi hai tác vụ xuất nhãn chạy chồng thời gian. Thu thập bằng chứng, kiểm tra giả thuyết và sửa sao cho mỗi yêu cầu giữ đúng dữ liệu của mình, kể cả khi thứ tự hoàn thành khác thứ tự bắt đầu.

## Bối cảnh thực tế

Một ngân hàng hạt giống khu vực gửi mẫu đến hai kho lưu trữ. Dịch vụ .NET tạo nhãn vận chuyển sau khi nhận phí tuyến từ một hệ thống nội bộ. Trong kiểm thử đơn lẻ, nhãn luôn chính xác. Khi hai yêu cầu đến gần nhau, một nhãn đôi khi mang thông tin của lô khác. Đây là dữ liệu giả lập, không gọi dịch vụ thật.

## Bạn cần làm gì

1. Chạy kiểm thử tuần tự và kiểm thử chồng thời gian; ghi lại nội dung hai nhãn.
2. Vẽ timeline tại thời điểm mỗi yêu cầu bắt đầu, chờ phí và hoàn thành.
3. Đưa ra ít nhất hai giả thuyết, xác định bằng chứng loại trừ từng giả thuyết.
4. Sửa code trong `starter/` mà không đổi hợp đồng đầu ra, không loại bỏ xử lý bất đồng bộ.
5. Chạy verification và kiểm tra cả đường đi tuần tự lẫn chồng thời gian.

## Yêu cầu môi trường

- PowerShell 7 hoặc Windows PowerShell 5.1.
- .NET SDK 8.0 trở lên; project target `net8.0`, không dùng NuGet package bên ngoài.
- Không cần Docker, Azure, database hoặc network.

## Chạy nhanh

Từ thư mục của unit:

~~~powershell
./setup.ps1
./run.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề

Chạy `./reproduce.ps1` trên starter nguyên bản. Script **thành công (exit 0)** khi chứng minh được tình trạng sai lệch dự kiến; đây không phải kiểm thử xác nhận bản sửa. Chạy `./verify.ps1` trước khi sửa để thấy regression contract chưa đạt.

## Những gì cần quan sát

- Hợp đồng nhãn tuần tự có đạt không?
- Nhãn A và B chứa những `LotId`, `Facility`, `Route` và `Fee` nào?
- Kết quả thay đổi như thế nào khi yêu cầu B hoàn thành trước A?
- Hành vi có tái hiện ổn định mà không phụ thuộc tốc độ CPU không?

Ghi bằng chứng vào [workspace/my-investigation.md](workspace/my-investigation.md). Có thể đọc [evidence/observations.md](evidence/observations.md) sau lần chạy đầu tiên.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự điều tra:** [solution/README.md](solution/README.md).

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md).

## Estimated Time

40–55 phút (L2 Investigation, D4).
