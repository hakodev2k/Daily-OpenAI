# UNIT-PIPEIO-001 — Worker kết xuất bảng điểm treo khi ghi log chẩn đoán

## Mục tiêu

Điều tra worker .NET gọi tiến trình con kết xuất bảng điểm PDF, phân biệt lỗi renderer với lỗi điều phối, và chứng minh bản sửa bằng kiểm thử cả thành công lẫn thất bại.

## Bối cảnh thực tế

Dịch vụ trường đại học gọi renderer qua command line. Với tài liệu nhỏ, worker trả kết quả bình thường. Khi bật chế độ chẩn đoán chi tiết, renderer thường bị worker kết luận quá thời hạn 4 giây; CPU không cao, không có exception từ renderer. Báo cáo bị xếp vào hàng chờ thử lại dù renderer chỉ ghi nhiều dữ liệu chẩn đoán. Môi trường mô phỏng dùng chính chương trình .NET làm renderer con, không cần phần mềm PDF hay dịch vụ trả phí.

## Bạn cần làm gì

- Tái hiện hai kích thước đầu ra, ghi exit code và thời gian.
- Thu thập bằng chứng về trạng thái tiến trình con, hai luồng dữ liệu và deadline.
- Viết tối thiểu ba giả thuyết trước khi xem hints.
- Sửa trong starter/Program.cs, giữ nguyên hợp đồng exit code và cleanup.
- Chạy verify.ps1 cho đầu ra nhỏ, lớn và lỗi renderer; giải thích vì sao chỉ tăng timeout không đủ.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK và runtime. global.json chọn SDK 8.0.100 với latestFeature roll-forward. Không cần NuGet ngoài framework.

## Chạy nhanh

Từ thư mục unit, chạy: pwsh -File ./scripts/reproduce.ps1

Hoặc Windows PowerShell: powershell -File ./scripts/reproduce.ps1

Chạy trực tiếp: pwsh -File ./scripts/run.ps1

## Cách reproduce vấn đề

reproduce.ps1 build starter/, chạy đầu ra nhỏ (128 ký tự), rồi đầu ra lớn (524288 ký tự). Script chỉ PASS khi case nhỏ hoàn tất và case lớn biểu hiện triệu chứng dự kiến. PASS ở đây không có nghĩa worker đã được sửa.

## Những gì cần quan sát

Ghi lại marker, exit code, stdout, stderr và trạng thái chờ. Đọc [bằng chứng sự cố mô phỏng](evidence/incident-snapshot.md). Không kết luận từ một log riêng lẻ.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter/.
4. Verify bằng pwsh -File ./scripts/verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference Solution — chỉ xem sau khi tự điều tra](solution/README.md).

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). verify.ps1 phải chạy starter/ đã chỉnh sửa, không chạy bản reference.

## Estimated Time

55 phút: 10 phút reproduce, 15 phút điều tra, 20 phút sửa và verify, 10 phút đối chiếu.
