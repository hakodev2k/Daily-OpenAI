# UNIT-GATEOWN-001 — Scanner gate khi request bị hủy

## Mục tiêu
Điều tra giới hạn concurrency trong .NET worker khi request bị hủy trước khi được phục vụ.

## Bối cảnh thực tế
Kho vận chỉ cho phép một lần cập nhật firmware scanner tại một thời điểm. Khi một yêu cầu bị hủy trong lúc chờ, dashboard đôi khi báo hai thiết bị được cập nhật đồng thời.

## Bạn cần làm gì
Chạy chương trình, thu thập thứ tự event, xác định invariant bị vi phạm, sửa code trong `starter/Program.cs` và chứng minh sửa đúng.

## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần package, Docker hoặc cloud.

## Chạy nhanh
```powershell
dotnet run --project starter/ScannerGate.csproj
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
Theo dõi `waiting`, `cancelled`, `entered`, `MAX_ACTIVE`. So sánh với giới hạn một tác vụ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md).

## Expected Results
Trước sửa: `MAX_ACTIVE=2`. Sau sửa: `MAX_ACTIVE=1` và yêu cầu bị hủy không đi vào vùng bảo vệ.

## Estimated Time
40 phút.
