# UNIT-CQCLAIM-001 — Phiếu nhận sách liên chi nhánh bị trùng và thất lạc

## Mục tiêu
Điều tra một sai lệch nghiệp vụ xuất hiện khi hai worker cùng xử lý hàng đợi yêu cầu nhận sách. Tái hiện bằng fixture có kiểm soát, thu thập evidence, phân biệt các giả thuyết cạnh tranh và sửa đường xử lý mà không làm mất yêu cầu hợp lệ.

## Bối cảnh thực tế
Hệ thống thư viện công cộng điều phối các yêu cầu nhận sách từ chi nhánh này sang chi nhánh khác. Mỗi yêu cầu chỉ được cấp một phiếu nhận. Trong một đợt tăng tải, hai worker báo đã hoàn tất hai lượt xử lý, nhưng hệ thống in hai phiếu cho cùng một yêu cầu và không có phiếu cho yêu cầu còn lại. CPU và bộ nhớ đều bình thường; hệ thống không ghi exception.

## Bạn cần làm gì
1. Chạy fixture và lưu ID đầu vào, ID đã xử lý, số bản ghi trùng và số yêu cầu bị thiếu.
2. Đưa ra ít nhất ba giả thuyết (nguồn dữ liệu, khâu nhận việc, khâu ghi phiếu) và chọn bằng chứng để bác bỏ từng giả thuyết.
3. Chỉ chỉnh sửa `starter/PickupDispatcher.cs`. Giữ nguyên public API, không sửa `harness/` hay fixtures.
4. Chạy verify cho cả hai thứ tự yêu cầu, một yêu cầu và hàng đợi rỗng.
5. So sánh với solution sau khi tự sửa và giải thích giới hạn của bảo đảm này khi chuyển sang queue phân tán.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 5.1+ hoặc PowerShell 7. Không cần database, Docker, cloud hoặc NuGet ngoài SDK.

## Chạy nhanh
Tại thư mục unit:
```powershell
pwsh -File ./setup.ps1
pwsh -File ./reproduce.ps1
pwsh -File ./run.ps1
pwsh -File ./verify.ps1
```
Trên Windows PowerShell dùng `powershell -ExecutionPolicy Bypass -File .\reproduce.ps1`.

## Cách reproduce vấn đề
`reproduce.ps1` chạy hai fixture có điểm đồng bộ để tái hiện cùng một lịch xen kẽ, rồi so sánh phiếu đã xử lý với danh sách yêu cầu. Exit code 0 nghĩa là **triệu chứng gốc đã được tái hiện**, không có nghĩa code đã đúng. `verify.ps1` phải thất bại trước khi sửa.

## Những gì cần quan sát
- Hai worker có hoàn tất mà không có exception không?
- Tập ID đầu ra có bằng tập ID đầu vào (kể cả số lần xuất hiện) không?
- Sai lệch có phụ thuộc thứ tự yêu cầu hay thời gian chờ không?
- Khi chỉ còn một yêu cầu hoặc hàng đợi rỗng, hành vi có ổn định không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence.
3. Tự sửa trong `starter/`.
4. Verify hành vi và regression.
5. Chỉ sau đó xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — không xem trước khi thử sửa.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)
- `reproduce.ps1` exit 0 khi thấy đúng triệu chứng starter; sau sửa đúng nó sẽ exit 1.
- `verify.ps1` exit 0 chỉ khi **code đang nằm trong starter** đáp ứng hợp đồng.

## Estimated Time
45–55 phút (L2 Investigation, D4).
