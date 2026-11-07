# UNIT-BILL-001 — Sai lệch đối soát hóa đơn usage-based billing

## Mục tiêu
Điều tra sai lệch settlement của một billing service .NET: reproduce, thu thập evidence, kiểm chứng giả thuyết, sửa và verify.

## Bối cảnh thực tế
Một SaaS platform nhận raw decimal usage charges, có credit âm. Một số hóa đơn lệch 0,01–0,02 so với partner settlement dù event count/checksum không đổi. API health vẫn tốt. Hợp đồng quy định: **cộng raw charge của toàn hóa đơn, rồi làm tròn thành 2 chữ số thập phân theo MidpointRounding.AwayFromZero**. Không được sửa raw audit data.

## Bạn cần làm gì
1. Reproduce và ghi nhận ít nhất ba invoice sai.
2. Đọc `evidence/`, đưa hai giả thuyết và cách loại trừ.
3. Chỉ chỉnh `starter/InvoiceCalculator.cs`; không chỉnh fixtures, runner hoặc reference.
4. Verify trên starter đã sửa, gồm credit, empty, null, grouping invariant.
5. Viết kết luận và production implications vào workspace.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 5.1+ hoặc 7+. Không cần Docker, Azure, database hay NuGet bên ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit 0 là **chứng minh triệu chứng của starter**; sau khi sửa dùng `./verify.ps1`.

## Những gì cần quan sát
Hóa đơn nào lệch? Raw sum, local total, settlement total khác nhau khi nào? Credit có phải điều kiện bắt buộc? Cùng tổng raw nhưng chia event khác nhau có thay đổi settlement không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md) · [Investigation workspace](workspace/my-investigation.md).

## Estimated Time
55–70 phút.
