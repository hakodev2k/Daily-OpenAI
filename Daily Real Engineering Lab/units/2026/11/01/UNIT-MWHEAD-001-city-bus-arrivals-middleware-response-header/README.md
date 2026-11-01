# UNIT-MWHEAD-001 — Header truy vết biến mất sau khi API trả dữ liệu

## Mục tiêu
Reproduce lỗi HTTP contract trong ASP.NET Core, quan sát response thực và kiểm chứng bản sửa bằng cùng probe trên code của bạn.

## Bối cảnh thực tế
API giờ xe buýt trả JSON đúng nhưng thiếu mã truy vết request theo hợp đồng với trung tâm điều hành. Đôi khi telemetry báo lỗi sau khi client đã nhận dữ liệu.

## Bạn cần làm gì
Chạy cả hai endpoint, ghi status/body/headers; đưa ra hai hypothesis; sửa `starter/Program.cs` mà không đổi payload; kiểm tra header của hai request khác nhau.

## Yêu cầu môi trường
.NET 8 SDK/runtime, PowerShell 5.1+ hoặc PowerShell 7. Không cần Azure, Docker, database, package bên ngoài. Kestrel bind `127.0.0.1` port động.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter và exit 0 khi probe phát hiện `OBSERVED_FAILURE`; sau khi sửa, nó phải exit 2. `verify.ps1` chỉ exit 0 khi code trong `starter/` đạt hợp đồng.

## Những gì cần quan sát
HTTP 200, body JSON nguyên vẹn, `X-Request-Trace` của mỗi response, ngoại lệ/transport failure nếu có. Không dùng latency làm chỉ tiêu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./scripts/verify.ps1`.
5. Sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
Trước: `OBSERVED_FAILURE`, thiếu header hoặc transport failure; reproduce exit 0, verify exit 3. Sau: `CONTRACT_OK`, hai response status 200, JSON đúng, mỗi response một header không rỗng và khác nhau; verify exit 0, reproduce exit 2. Nếu không reproduce: kiểm tra starter chưa sửa, runtime .NET 8 và quyền loopback.

## Estimated Time
30–40 phút (L1 / D3).
