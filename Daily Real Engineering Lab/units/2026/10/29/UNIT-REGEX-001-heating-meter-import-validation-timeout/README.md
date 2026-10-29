# UNIT-REGEX-001 — Một meter ID khiến batch import ngừng xử lý

## Mục tiêu
- Reproduce một lỗi chỉ xuất hiện khi dữ liệu đầu vào có hình dạng bất thường.
- Phân biệt lỗi nghiệp vụ, lỗi hạ tầng và chi phí thực thi của một bước validation.
- Đưa ra bản sửa có thể đo và kiểm tra bằng regression cases, không làm thay đổi quy tắc ID.

## Bối cảnh thực tế
Một đơn vị vận hành mạng sưởi khu vực nhận danh sách meter ID từ gateway. Đa số bản ghi được kiểm tra ngay, nhưng một ID do đối tác gửi khiến worker hết thời gian xử lý validation. Các bản ghi tiếp theo trong batch không được xử lý, dù CPU và database bình thường ở các batch khác.

## Bạn cần làm gì
1. Chạy starter và ghi lại input nào khác biệt, kết quả từng case và exception.
2. Nêu ít nhất hai hypothesis trước khi đọc hints.
3. Chỉ sửa `starter/MeterCodeValidator.cs`; giữ nguyên hợp đồng: ID không rỗng, chỉ gồm chữ in hoa ASCII A–Z và chữ số 0–9.
4. Chạy verify để chứng minh ID hợp lệ vẫn được chấp nhận, ID không hợp lệ bị từ chối, và input dài không làm hỏng batch.

## Yêu cầu môi trường
- .NET SDK 8.x và PowerShell 7 hoặc Windows PowerShell 5.1.
- Không cần Azure, Docker, database hoặc NuGet ngoài SDK.

## Chạy nhanh
Chạy trong thư mục unit:
~~~powershell
./setup.ps1
./run.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
`./reproduce.ps1` chạy bản starter nguyên gốc và chỉ trả exit code 0 nếu triệu chứng mong đợi được ghi nhận. Dòng `EXPECTED_FAILURE_REPRODUCED` là bằng chứng reproduce thành công, **không** có nghĩa là validation đã đúng.

## Những gì cần quan sát
- Những meter ID thông thường được chấp nhận hoặc từ chối thế nào?
- ID có một ký tự cuối không hợp lệ gây triệu chứng khác ra sao?
- Worker báo lỗi ở bước nào, và vì sao các bản ghi còn lại không tiếp tục?
- Có thể giữ nguyên quy tắc nghiệp vụ mà vẫn xử lý input bất thường trong thời gian hữu hạn không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `./verify.ps1` trên **starter đã sửa**.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [Reference Solution](solution/README.md) sau khi tự điều tra và sửa.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
30–40 phút. Ghi kết quả vào [My Investigation](workspace/my-investigation.md).
