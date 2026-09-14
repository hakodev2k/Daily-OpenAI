# UNIT-DEPLOY-004 — Readiness Before Warmup

## Mục tiêu

Điều tra một ASP.NET Core service được orchestration platform đưa traffic vào quá sớm sau khi process khởi động.

## Bối cảnh thực tế

Một catalog service có bước warmup dữ liệu nền mất vài giây. Deployment báo instance healthy gần như ngay lập tức, nhưng các request nghiệp vụ đầu tiên đôi lúc nhận `503 Service Unavailable`.

## Bạn cần làm gì

- Reproduce khoảng thời gian instance được báo sẵn sàng nhưng endpoint nghiệp vụ chưa phục vụ được.
- Ghi ít nhất 2 hypothesis.
- Xác định contract phù hợp giữa liveness, readiness và trạng thái warmup.
- Sửa code trong `starter/`.
- Chạy `verify.ps1` để kiểm tra cả giai đoạn trước và sau warmup.
- Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

Script khởi động service trên local port, gọi health endpoints ngay sau startup và so sánh chúng với khả năng phục vụ `/catalog/count`.

## Những gì cần quan sát

- HTTP status của `/health/live`.
- HTTP status của `/health/ready` ngay sau startup.
- HTTP status của `/catalog/count` trong cùng thời điểm.
- Trạng thái các endpoint sau khi warmup hoàn tất.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix.

[Solution](solution/README.md)

## Expected Results

Before:
- Process sống và liveness trả thành công.
- Có một cửa sổ thời gian health signal cho phép nhận traffic trong khi business endpoint chưa sẵn sàng.

After:
- Liveness vẫn biểu diễn process health.
- Readiness chỉ thành công khi instance thực sự có thể phục vụ request cần warmup.
- Sau warmup, business endpoint và readiness đều thành công.

## Estimated Time

30–45 phút.
