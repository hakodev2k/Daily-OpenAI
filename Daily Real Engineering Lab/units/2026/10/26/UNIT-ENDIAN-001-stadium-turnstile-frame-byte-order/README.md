# UNIT-ENDIAN-001 — Turnstile telemetry: số lượt vào sân bất thường

## Mục tiêu
Điều tra binary telemetry trong .NET, thu thập raw evidence, so sánh hợp đồng thiết bị và chứng minh fix bằng regression checks.

## Bối cảnh thực tế
Turnstile sân vận động lưu lượt vào cổng khi mất mạng rồi đồng bộ các frame 8 byte. Dashboard đôi khi ghi nhận gate không tồn tại và lượt vào lớn bất thường. Thiết bị khẳng định frame hợp lệ.

## Bạn cần làm gì
1. Chạy starter, ghi lại raw bytes và giá trị decode.
2. Đọc [device contract](evidence/device-contract.md), đưa ra ít nhất hai hypothesis.
3. Sửa **starter/GateFrameDecoder.cs**; không sửa fixture để che triệu chứng.
4. Verify cả positive và negative fixtures.
5. So sánh [reference solution](solution/README.md) sau khi tự thử.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần Docker, NuGet ngoài .NET SDK hay dịch vụ trả phí. Xem `global.json`.

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```
Tương đương với dotnet CLI:
```bash
dotnet run --project starter/GateFrameLab.csproj -- --inspect
dotnet run --project starter/GateFrameLab.csproj -- --reproduce
dotnet run --project starter/GateFrameLab.csproj -- --verify
```

## Cách reproduce vấn đề
`reproduce.ps1` trả exit code 0 và in `REPRODUCED` khi starter còn triệu chứng. `verify.ps1` trả exit code 1 trước khi sửa; chỉ trả 0 khi decoder thỏa hợp đồng.

## Những gì cần quan sát
Đối chiếu gate ID, entry count và raw frame với tài liệu vendor. Tách giả thuyết về frame length, version, field offset và cách giải mã số.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) chỉ đọc sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút — L2 / D4.
