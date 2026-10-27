# UNIT-CRYPTO-001 — Bản xuất kho lạnh được mã hóa nhưng vẫn lộ quan hệ giữa hai hồ sơ

## Mục tiêu
Điều tra tính bảo mật của encrypted envelope trong .NET bằng chứng cứ byte-level; phân biệt giải mã thành công với confidentiality; xác minh sửa đổi không làm hỏng authentication và round-trip.

## Bối cảnh thực tế
Nền tảng kho lạnh xuất nhiều bản ghi kiểm định nhiệt độ trong cùng một batch. Mỗi bản ghi được mã hóa bằng AES-GCM trước khi gửi bộ phận compliance. Đội kiểm thử nhận thấy hai bản ghi có thể để lộ quan hệ giữa nội dung gốc dù người quan sát không có key. Harness dùng dữ liệu synthetic hoàn toàn local.

## Bạn cần làm gì
1. Chạy starter, lưu evidence và ít nhất hai giả thuyết.
2. Điều tra các envelope cùng batch và đối chiếu với khác batch.
3. Chỉ sửa starter/EnvelopeCrypto.cs; không sửa harness hay scripts.
4. Giữ hợp đồng: decrypt đúng nội dung; sai batch, sai key, ciphertext hoặc tag bị sửa đều phải bị từ chối.
5. Chạy verification và giải thích tại sao round-trip test đơn thuần chưa đủ.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 5.1+ hoặc 7+. Không cần Docker, database, NuGet ngoài SDK, mạng hay cloud.

## Chạy nhanh
~~~powershell
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Exit code 0 nghĩa là harness đã xác nhận triệu chứng gốc; build lỗi không được tính là reproduce.

## Những gì cần quan sát
- Những trường nào của hai encrypted envelope giống hoặc khác nhau?
- Dữ liệu quan sát được tiết lộ quan hệ giữa hai plaintext hay không?
- Vì sao cả hai bản ghi vẫn decrypt và authenticate thành công?
- Kiểm thử nào phân biệt được lỗi bảo mật với round-trip thành công?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter/.
4. Verify bằng ./verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ xem sau khi tự điều tra và thử fix.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
70 phút · L3 / D4 · Validation: static-review (code chưa được chạy trong môi trường tạo lab).
