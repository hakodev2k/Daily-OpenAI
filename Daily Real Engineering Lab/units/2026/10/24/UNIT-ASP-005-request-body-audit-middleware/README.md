# UNIT-ASP-005 — Audit middleware khiến shipment API từ chối payload hợp lệ

## Mục tiêu
Điều tra request pipeline ASP.NET Core bằng evidence, sửa middleware và xác minh không hồi quy audit hoặc JSON validation.

## Bối cảnh thực tế
API nhận sự kiện vận chuyển từ nhà cung cấp vừa bật audit middleware ghi số byte request (không lưu nội dung). Từ đó nhiều shipment JSON hợp lệ trả 400, trong khi payload sai vẫn phải bị từ chối. Nhóm cần giữ nguyên audit và không làm suy yếu API contract.

## Bạn cần làm gì
1. Chạy starter, ghi status/body/auditBytes cho bốn request.
2. Đề xuất ít nhất hai giả thuyết, kiểm tra bằng chứng tại middleware và endpoint.
3. Chỉ sửa **starter/RequestAuditMiddleware.cs**, không sửa runner hoặc endpoint.
4. Chạy verify trên starter bạn đã sửa rồi mới so sánh reference.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 7+ hoặc Windows PowerShell 5.1; không cần database, Docker, Azure hay NuGet ngoài. Runner dùng DefaultHttpContext và forward-only stream, không mở HTTP listener thật.

## Chạy nhanh
~~~powershell
./setup.ps1
./run.ps1
~~~
Hoặc: dotnet run --project starter/AuditLab.Starter.csproj -- run

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Exit code 0 của reproduce xác nhận **triệu chứng gốc**, không xác nhận fix. Sau khi chỉnh sửa:
~~~powershell
./verify.ps1
~~~
Verify chạy **starter đang chỉnh sửa**, không chạy solution.

## Những gì cần quan sát
- Status và body của hai shipment hợp lệ.
- Status của malformed JSON và body rỗng.
- auditBytes trước/sau sửa; số byte audit ghi nhận có đồng nghĩa endpoint đọc được payload không?
- Có request nào bị chấp nhận sai sau khi sửa không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: chỉ mở [Reference Solution](solution/README.md) sau khi đã tự điều tra và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
65 phút. Ghi suy luận tại [workspace](workspace/my-investigation.md).
