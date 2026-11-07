# UNIT-CACHE-001 — Schema cache đúng kết quả nhưng lãng phí biên dịch dưới tải đồng thời

## Mục tiêu
Điều tra một cache khi nhiều request đồng thời yêu cầu schema chưa được chuẩn bị, đo công việc thực tế thay vì chỉ kiểm tra kết quả trả về và thiết kế regression test.

## Bối cảnh thực tế
API biểu mẫu nhiều tenant biên dịch schema validation theo phiên bản. Sau deployment, request trả về dữ liệu hợp lệ nhưng CPU và số lần compile tăng bất thường trong các đợt cold-start. p95 xấu đi dù cache cuối cùng vẫn có một bản cho mỗi key.

## Bạn cần làm gì
1. Chạy baseline và ghi lại các chỉ số trước khi sửa.
2. Đưa ra ít nhất hai giả thuyết về lý do nhiều công việc xảy ra cho cùng một key.
3. Chỉ sửa `starter/SchemaCache.cs`, giữ nguyên public API và key isolation.
4. Chạy `verify.ps1` trên starter do bạn sửa, sau đó mới xem reference.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 5.1+ hoặc 7+. Không cần Docker, Redis, cloud hay NuGet ngoài framework.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script dùng baseline bất biến để tái hiện triệu chứng gốc. Exit code 0 ở bước này nghĩa là **reproduced**, không phải đã fix. Có thể chạy trực tiếp: `dotnet run --project baseline/CacheFactoryLab.Baseline.csproj -- reproduce`.

## Những gì cần quan sát
- `FirstKeyCompileCount` khi tám worker cùng khởi động.
- `AllWorkersShareCachedInstance` và `AllResultsMatchKey`.
- Phân biệt công việc đã chạy với giá trị được giữ lại trong cache.
- Key thứ hai có còn độc lập sau khi sửa hay không.

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
⚠️ Spoiler: chỉ mở [Reference Solution](solution/README.md) sau khi tự thử và verify.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
55 phút. Ghi evidence và hypotheses trong [workspace](workspace/my-investigation.md).
