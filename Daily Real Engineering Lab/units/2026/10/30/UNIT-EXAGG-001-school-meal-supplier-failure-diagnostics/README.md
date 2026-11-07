# UNIT-EXAGG-001 — Báo cáo nhà cung cấp bữa ăn học đường bỏ sót lỗi song song

## Mục tiêu
Tái hiện một báo cáo vận hành không phản ánh đầy đủ các nguồn dữ liệu thất bại, thu thập bằng chứng có thể kiểm chứng, sau đó sửa đường xử lý kết quả mà không làm sai kết quả thành công hoặc ngữ nghĩa hủy tác vụ.

## Bối cảnh thực tế
Mỗi đêm, nền tảng bữa ăn học đường tổng hợp số lượng mặt hàng từ ba nhà cung cấp để kiểm tra hồ sơ dị ứng. Trong một lần gián đoạn, hai nhà cung cấp báo lỗi độc lập nhưng bảng vận hành chỉ ghi một lỗi. Một lượt chạy bị hủy cũng bị hiển thị như một lỗi tích hợp thông thường. Điều này khiến đội vận hành xử lý thiếu đầu mối và nhầm lẫn giữa sự cố và hủy theo yêu cầu.

## Bạn cần làm gì
1. Chạy mẫu, ghi lại số lượng nguồn lỗi thật và số lỗi xuất hiện trong kết quả.
2. Đưa ra ít nhất hai giả thuyết về nơi bằng chứng bị mất; kiểm tra riêng trường hợp chỉ có một lỗi, nhiều lỗi và hủy.
3. Chỉ sửa `starter/SupplierReport.cs`, giữ nguyên public API và không sửa harness.
4. Chạy verify, ghi kết quả trước/sau và đề xuất cách đưa chẩn đoán lên telemetry production mà không lộ thông tin nhạy cảm.

## Yêu cầu môi trường
- .NET SDK 8.x (xem `global.json`); không cần database, Docker, tài khoản cloud hay NuGet ngoài SDK/framework.
- PowerShell 5.1+ hoặc PowerShell 7.

## Chạy nhanh
Từ thư mục unit:
```powershell
pwsh -File ./setup.ps1
pwsh -File ./reproduce.ps1
pwsh -File ./run.ps1
pwsh -File ./verify.ps1
```
Trên Windows PowerShell có thể dùng `powershell -ExecutionPolicy Bypass -File .\reproduce.ps1`.

## Cách reproduce vấn đề
`reproduce.ps1` chạy ba fixture độc lập: nguồn bình thường, hai nguồn thất bại cùng lượt, và một lượt bị hủy. Exit code 0 nghĩa là **triệu chứng của starter đã được tái hiện**; không có nghĩa hệ thống đã đúng. `verify.ps1` trước khi sửa phải thất bại.

## Những gì cần quan sát
- Khi hai nguồn báo lỗi, dashboard thực sự giữ lại bao nhiêu chẩn đoán?
- Kết quả tổng hợp có vô tình được công bố dù thiếu nguồn hay không?
- Trường hợp hủy có giữ được tín hiệu hủy riêng với lỗi tích hợp?
- Sau sửa, các fixture bình thường có còn giữ nguyên tổng số lượng và trạng thái công bố?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong `starter/`.
4. Verify cả các nhánh lỗi, hủy và thành công.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ xem sau khi đã tự điều tra.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)
- `reproduce.ps1`: exit 0 khi phát hiện triệu chứng gốc.
- `verify.ps1`: exit 0 chỉ khi code người học trong `starter/` đáp ứng toàn bộ hợp đồng.

## Estimated Time
35–45 phút (L1 deterministic engineering failure, D3).
