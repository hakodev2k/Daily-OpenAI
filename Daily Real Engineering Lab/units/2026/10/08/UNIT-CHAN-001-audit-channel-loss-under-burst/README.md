# UNIT-CHAN-001 — Audit events disappear during traffic bursts

## Mục tiêu
Điều tra sự chênh lệch giữa số sự kiện được API tiếp nhận và số sự kiện đi qua pipeline xử lý; đưa ra thay đổi có thể kiểm chứng về correctness, giới hạn tài nguyên và áp lực tải.

## Bối cảnh thực tế
Một dịch vụ quản trị ghi nhận sự kiện thay đổi quyền truy cập. Khi quản trị viên thao tác hàng loạt, API báo đã tiếp nhận 12 sự kiện nhưng hệ thống downstream chỉ nhìn thấy một phần. Không có exception; CPU và bộ nhớ ổn định. Sự kiện audit không được phép mất âm thầm.

## Bạn cần làm gì
1. Chạy bản starter, lưu số lượng và thứ tự sự kiện quan sát được.
2. Viết tối thiểu hai giả thuyết, phân biệt lỗi consumer, lỗi producer và chính sách hàng đợi.
3. Sửa mã trong `starter/AuditPipeline.cs`; không chỉnh test harness để qua bài.
4. Chạy verify và giải thích hành vi khi consumer chậm hoặc dừng.
5. Đánh giá thêm rủi ro mất dữ liệu khi tiến trình bị crash (ngoài phạm vi bảo đảm của bài).

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7.
- .NET 8 SDK (không cần database, Docker, Azure hay NuGet package ngoài framework).

## Chạy nhanh
```powershell
cd "Daily Real Engineering Lab/units/2026/10/08/UNIT-CHAN-001-audit-channel-loss-under-burst"
./scripts/setup.ps1
./scripts/reproduce.ps1
# Sửa starter/AuditPipeline.cs rồi:
./scripts/verify.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên bản starter ban đầu. Script kết thúc thành công khi xác nhận được triệu chứng ban đầu. Nếu đã sửa starter, xem output từ lần reproduce trước; script không phải bài kiểm tra sau sửa.

## Những gì cần quan sát
Đối chiếu `published`, `observed`, danh sách sequence, lỗi producer/consumer và thời điểm producer kết thúc khi consumer chưa sẵn sàng. Dữ liệu gợi ý bổ sung trong [evidence/incident.md](evidence/incident.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify; giữ nguyên harness.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra và verify.

## Expected Results
- Trước: reproduce xác nhận sự kiện bị thiếu nhưng producer không báo lỗi.
- Sau: verify yêu cầu nhận đủ 12 sequence đúng thứ tự, đồng thời chứng minh producer phải chịu áp lực khi consumer tạm dừng; không được dùng hàng đợi vô hạn để vượt bài.

## Estimated Time
60–75 phút (L3 / D4).

