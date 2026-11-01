# UNIT-REDIR-001 — Biên nhận ảnh kiểm định thang máy báo thành công nhưng đối tác không lưu

## Mục tiêu
Điều tra một ca **HTTP 2xx giả thành công** trong luồng chuyển chứng cứ kiểm định sang hệ thống đối tác. Thu thập wire-level evidence, phân biệt transport success với business acknowledgement, rồi sửa và kiểm chứng contract.

## Bối cảnh thực tế
Đội kiểm định thang máy chung cư tải ảnh biên bản lên cổng đối tác. Dashboard nội bộ hiển thị "đã gửi" nhưng nhân viên đối tác không tìm thấy hồ sơ. Tỷ lệ HTTP thành công vẫn 100%; một thay đổi đường dẫn ở đối tác vừa được triển khai.

## Bạn cần làm gì
1. Chạy mô phỏng partner trên loopback, tái hiện hai lần gửi.
2. Ghi lại method, URL, status, Content-Type và số biên nhận thật sự được lưu.
3. Đưa ra ít nhất hai hypothesis, chỉ ra bằng chứng bác bỏ/ủng hộ.
4. Sửa **chỉ** `starter/PartnerGateway.cs`, giữ chính sách không tự động tin vào redirect hoặc HTTP 200.
5. Chạy verify, sau đó so sánh với reference solution.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK. Không cần Azure, database, Docker hoặc tài khoản bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```
`reproduce.ps1` thành công (exit 0) khi phát hiện starter vi phạm contract; không có nghĩa hệ thống đã được sửa.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`, quan sát hai request bắt đầu ở endpoint legacy. Xem `evidence/incident.log` để đối chiếu timeline và response metadata. Nếu cần, chạy `dotnet run --project ./starter/EvidenceHandoff.csproj -- inspect`.

## Những gì cần quan sát
- Trạng thái ứng dụng tự báo cho mỗi lần gửi so với số evidence đã lưu tại partner.
- HTTP method và endpoint cuối cùng, loại response, mã status và sự hiện diện của receipt ID.
- Phân biệt lỗi chuyển hướng, lỗi xác thực biên nhận và các chỉ số vận hành không liên quan.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/PartnerGateway.cs`.
4. Verify bằng `./scripts/verify.ps1` (exit 0 mới đạt).
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi đã thử sửa.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
65 phút (L3 / D4).