# UNIT-MSGRETRY-001 — Water Meter Alert Dispatch: Một thông báo lỗi làm tắc cả hàng đợi

## Mục tiêu
Điều tra một sự cố xử lý thông báo theo batch, tái hiện bằng dữ liệu deterministic, đưa ra quyết định về chính sách xử lý lỗi và chứng minh cả tính đúng đắn lẫn khả năng tiếp tục phục vụ các site độc lập.

## Bối cảnh thực tế
Hệ thống giám sát đồng hồ nước gửi cảnh báo cho các đội vận hành theo site. Một bản ghi không được gateway chấp nhận; dashboard cho thấy các site khác không nhận được thông báo dù worker vẫn hoạt động. Nhóm vận hành cần giữ thứ tự thông báo trong từng site, không để một site gây nghẽn toàn hệ thống và phải có đường xử lý thủ công cho thông báo không giao được.

## Bạn cần làm gì
1. Chạy starter và ghi lại timeline, số lần gửi, delivered/dead-letter.
2. Đưa ra ít nhất hai hypothesis từ evidence, chỉ ra impact và các giả định.
3. Đề xuất chính sách có giới hạn cho lỗi tạm thời/lỗi vĩnh viễn, fairness giữa site, ordering và vận hành dead-letter.
4. Sửa `starter/Program.cs` và chạy verification; ghi trade-off trong workspace.
5. Sau khi hoàn thành mới so sánh với reference solution.

## Yêu cầu môi trường
.NET SDK 8+ (có thể dùng SDK mới hơn theo `global.json`), PowerShell 7 hoặc Windows PowerShell. Không cần database, cloud hay NuGet bên thứ ba.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
./run.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter ở chế độ `--reproduce` và trả exit code 0 **chỉ khi** triệu chứng ban đầu thực sự xuất hiện. Sau khi sửa, dùng `./verify.ps1` để chạy ba hợp đồng hành vi trên mã learner-editable.

## Những gì cần quan sát
So sánh số attempts của P1, các message đã giao, thông báo chưa giao và thứ tự theo site. Đối chiếu với `evidence/incident-timeline.md`. Không suy ra thành công chỉ từ việc worker chạy hết tick budget.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi đã tự thử:** [Solution](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` phải PASS ba contract; `reproduce.ps1` chỉ dùng để xác nhận starter ban đầu.

## Estimated Time
65–90 phút, maturity L5, difficulty D4.
