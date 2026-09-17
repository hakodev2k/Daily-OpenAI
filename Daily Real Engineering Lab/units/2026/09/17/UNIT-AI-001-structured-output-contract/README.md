# UNIT-AI-001 — AI extraction trả JSON hợp lệ nhưng application vẫn xử lý sai

## Mục tiêu
Điều tra một integration failure khi downstream AI output nhìn có vẻ hợp lệ nhưng không đáp ứng contract mà business workflow cần.

## Bối cảnh thực tế
Một internal service dùng model để trích xuất thông tin ticket thành JSON rồi deserialize sang C# DTO. Hầu hết request chạy đúng. Một số response vẫn parse thành công nhưng workflow chọn sai priority hoặc thiếu dữ liệu bắt buộc, trong khi log không có exception.

## Bạn cần làm gì
1. Chạy starter và reproduce case sai.
2. Ghi hypothesis trước khi sửa.
3. So sánh transport validity, schema validity và business validity.
4. Thu thập evidence từ raw response và parsed object.
5. Sửa learner-editable code để integration boundary enforce contract phù hợp.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell
- Không cần API key hoặc dịch vụ AI bên ngoài; response được mô phỏng local.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- JSON có thể deserialize mà vẫn không đủ để workflow xử lý an toàn.
- Các case hợp lệ phải tiếp tục hoạt động.
- Case không đáp ứng contract phải được phát hiện ở boundary thay vì lan sang business logic.

## Expected Results
**Before:** một payload parse thành công nhưng dẫn đến quyết định business không hợp lệ.

**After:** integration boundary phân biệt được payload sử dụng được và payload cần reject/retry/fallback theo policy.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference Solution
Chỉ mở `solution/README.md` sau khi đã tự điều tra.

## Estimated Time
35–50 phút.