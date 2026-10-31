# UNIT-CHAN-001 — Nhật ký bàn giao hiện vật mất sự kiện trong đợt quét dồn

## Mục tiêu
Điều tra một pipeline audit không báo lỗi nhưng kết quả lưu trữ thiếu sự kiện. Phân biệt admission, xử lý và persistence; sửa theo yêu cầu không mất bản ghi khi lưu lượng tăng.

## Bối cảnh thực tế
Hệ thống bảo tàng ghi mỗi lần quét hiện vật khi chuyển kho. Sau một đợt nhập liệu dồn, dashboard ghi nhận 9 thao tác thành công nhưng kho audit chỉ thấy một phần. Không có exception; CPU và bộ nhớ bình thường. Đây là sự cố truy vết trách nhiệm, không chỉ là throughput.

## Bạn cần làm gì
Chạy baseline, ghi tối thiểu ba hypothesis, đối chiếu log và source, xác định ranh giới dữ liệu bị mất. Chỉ sửa **starter/AuditPipeline.cs**. Không sửa oracle trong **contract/Program.cs**.

## Yêu cầu môi trường
Windows PowerShell 5.1+/PowerShell 7 và .NET SDK 8+ có thể build target net8.0. Không cần Docker, database, cloud hoặc NuGet ngoài framework.

## Chạy nhanh
~~~powershell
./setup.ps1
./reproduce.ps1
./verify.ps1
~~~
Reproduce PASS khi phát hiện triệu chứng baseline; verify FAIL trước khi sửa và PASS sau khi sửa.

## Cách reproduce vấn đề
~~~powershell
./run.ps1
dotnet run --project starter/MuseumLoanAudit.csproj -- reproduce
~~~
Đối chiếu [evidence](evidence/incident-log.md) với [scenario](docs/scenario.md).

## Những gì cần quan sát
- Số event được chấp nhận so với số event được ghi.
- Các sequence thiếu có tạo exception hay không.
- Hành vi khi burst tăng từ 1 lên 9 rồi 40 event.
- Sự khác nhau giữa xử lý nhanh, sink chậm và sink lỗi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Chạy verify và đánh giá trade-offs.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
55–75 phút.
