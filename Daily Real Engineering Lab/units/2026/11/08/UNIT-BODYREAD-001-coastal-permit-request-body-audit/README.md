# UNIT-BODYREAD-001 — Coastal permit request audit

## Mục tiêu
Điều tra hợp đồng đọc request body giữa middleware và endpoint; sửa lỗi mà không làm mất audit hoặc thay đổi validation.

## Bối cảnh thực tế
Dịch vụ tiếp nhận giấy phép xây dựng ven biển bổ sung middleware ghi audit payload. Sau release, một số request JSON hợp lệ bị trả về 422 dù audit vẫn báo nhận đủ bytes.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai giả thuyết, xác định nơi dữ liệu biến mất, sửa `starter/Program.cs` và chứng minh hai trường hợp hợp lệ/không hợp lệ đều đúng.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 7 hoặc Windows PowerShell; không cần database, Azure hay NuGet bên thứ ba.

## Chạy nhanh
Từ thư mục unit: `pwsh -File ./reproduce.ps1` (Windows PowerShell: `powershell -File ./reproduce.ps1`).

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script yêu cầu chương trình starter chứng minh ít nhất một regression và sẽ báo PASS khi tái hiện được regression.

## Những gì cần quan sát
So sánh `expected`, `actual`, `audit.bytes` cho cả hai input; ghi lại điểm nào trong pipeline thay đổi kết quả.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify với `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md), [code](solution/Program.cs). Không mở trước khi tự thử.

## Expected Results
Trước sửa: valid-permit FAIL, missing-permit PASS. Sau sửa: cả hai PASS; audit vẫn ghi số bytes thực tế. `verify.ps1` phải exit 0.

## Estimated Time
40 phút.
