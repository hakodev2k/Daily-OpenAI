# UNIT-IPTRUST-001 — Callback hiệu chuẩn thiết bị bị chấp nhận sai nguồn IP

## Mục tiêu
Điều tra một sự cố kiểm soát nguồn request tại gateway .NET: tái hiện hành vi, phân loại bằng chứng từ connection và HTTP headers, đề xuất sửa đổi, kiểm chứng cả trường hợp hợp lệ lẫn không hợp lệ.

## Bối cảnh thực tế
Một nền tảng hiệu chuẩn thiết bị nhận callback từ đối tác thông qua một reverse proxy nội bộ. Đối tác phản ánh có request hợp lệ bị từ chối; đội an ninh đồng thời ghi nhận một callback được chấp nhận dù connection đến từ mạng không được cấp quyền. Quy định hiện tại: callback phải đi qua **đúng một proxy được quản lý**, và địa chỉ nguồn do proxy chuyển tiếp phải thuộc allowlist đối tác.

## Bạn cần làm gì
1. Chạy fixture hiện có và tái hiện trường hợp chấp nhận request sai nguồn.
2. Đối chiếu địa chỉ của TCP peer với dữ liệu header trong evidence; ghi ít nhất ba hypothesis.
3. Sửa duy nhất `starter/GatewayAuthorizer.cs` mà không thay đổi public method signature.
4. Kiểm tra request hợp lệ, request giả mạo, header rỗng/sai định dạng và chuỗi nhiều hop.
5. Giải thích điều kiện nào khiến giải pháp local này **không** đủ để đưa vào production.

## Yêu cầu môi trường
.NET 8 SDK (có .NET 8 runtime) và PowerShell trên Windows. Không cần Azure, Docker, database hoặc NuGet package ngoài SDK. Đây là mô phỏng xác định của chính sách gateway, không phải một reverse proxy thật.

## Chạy nhanh
```powershell
cd "Daily Real Engineering Lab/units/2026/10/30/UNIT-IPTRUST-001-calibration-callback-proxy-source-trust"
./setup.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` kiểm tra fixture tấn công đã biết. Exit code 0 nghĩa là **đã tái hiện lỗi**, không phải hệ thống đã an toàn. `./run.ps1` in toàn bộ kết quả để điều tra. Sau khi sửa `starter/`, dùng `./verify.ps1`.

## Những gì cần quan sát
So sánh `RemotePeer`, `X-Forwarded-For`, quyết định `ALLOW/DENY`, số fixture không khớp với policy. [Incident evidence](evidence/incident-log.md) và [workspace](workspace/my-investigation.md) hỗ trợ ghi nhận.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi tự điều tra và sửa:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` luôn chạy `starter/` đã được người học chỉnh sửa, không chạy reference.

## Estimated Time
55 phút.
