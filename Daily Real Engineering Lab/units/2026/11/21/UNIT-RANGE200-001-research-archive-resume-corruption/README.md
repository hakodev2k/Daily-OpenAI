# UNIT-RANGE200-001 — Resume tệp lưu trữ nghiên cứu gây sai checksum

## Mục tiêu
Điều tra tính toàn vẹn của bản tải tiếp qua HTTP dựa trên bằng chứng thay vì chỉ dựa vào status thành công.

## Bối cảnh thực tế
Hệ thống mirror tệp nghiên cứu tải tiếp sau khi gián đoạn. Một số lần tải có status thành công nhưng checksum sai; tải mới hoàn toàn thì đúng.

## Bạn cần làm gì
Chạy starter, ghi lại độ dài và status, đưa ra hai giả thuyết, sửa đường xử lý tải tiếp trong `starter/Program.cs` và verify.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell. Không cần mạng hay dịch vụ trả phí.

## Chạy nhanh
`dotnet run --project ./starter/ArchiveResume.csproj`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script xác nhận triệu chứng của bản starter.

## Những gì cần quan sát
So sánh offset, số byte nhận, kích thước cuối, expected và actual. Phân loại byte bị thêm, thiếu hay sai thứ tự; ghi bằng chứng vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md) chỉ xem sau khi tự thử.

## Expected Results
Trước sửa: reproduce thành công khi phát hiện lỗi, verify thất bại. Sau sửa: verify thấy `INTEGRITY_OK`.

## Estimated Time
40 phút — D4 / L2.
