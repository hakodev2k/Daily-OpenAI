# UNIT-MBUF-001 — Orchard sensor frame transport: bản tin bị từ chối

## Mục tiêu

Điều tra một lỗi tích hợp binary uplink trong .NET 8, phân biệt lỗi serialize dữ liệu với lỗi hợp đồng truyền tải, refactor có kiểm chứng và đánh giá chi phí copy bộ nhớ.

## Bối cảnh thực tế

Một hợp tác xã trồng cây ăn quả gửi các bản tin đo nhiệt độ chống sương giá từ thiết bị thu thập về gateway. Chạy cục bộ, từng bản tin chứa sensor ID và nhiệt độ hợp lệ; gateway lại từ chối mọi bản tin với lỗi `transport-length-mismatch`. Việc gửi lại không cải thiện tình hình. Dữ liệu nhiệt độ của vườn không cập nhật trên dashboard.

## Bạn cần làm gì

1. Chạy bản starter, ghi lại kích thước frame được gửi và kích thước khai báo.
2. Đề xuất ít nhất hai giả thuyết; dùng evidence để loại trừ giả thuyết sai.
3. Chỉ sửa `starter/FrameExporter.cs`, giữ nguyên giao thức và receiver.
4. Chạy verification cho cả ba sensor và một negative case.
5. Giải thích trade-off giữa copy an toàn và truyền theo một lát cắt bộ nhớ có quản lý lifetime.

## Yêu cầu môi trường

- .NET SDK 8.x (xem `global.json`), PowerShell 7 hoặc Windows PowerShell 5.1.
- Không cần Docker, Azure, database, NuGet package bên ngoài hoặc internet khi SDK đã sẵn có.

## Chạy nhanh

Từ thư mục unit:

```powershell
.\scripts\setup.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
dotnet run --project .\starter\OrchardFrameLab.csproj -- --reproduce
```

`reproduce.ps1` trả exit code 0 **chỉ khi** starter tái hiện đủ ba lần gateway từ chối. Sau khi tự sửa, chạy `.\scripts\verify.ps1` (hoặc `dotnet run --project .\starter\OrchardFrameLab.csproj -- --verify`). Verify luôn chạy mã **bạn đã sửa trong starter**, không chạy reference solution.

## Những gì cần quan sát

- Ba dòng `CASE`: trạng thái gateway, `sentBytes`, `declaredBytes`, và reason.
- Sensor ID, nhiệt độ đầu vào, thứ tự field và magic byte có nhất quán không?
- Gateway phân biệt lỗi transport với lỗi payload như thế nào?
- Sau sửa, ba case hợp lệ phải được chấp nhận; frame không hợp lệ vẫn phải bị từ chối.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi đã tự điều tra và verify:** [solution/README.md](solution/README.md)

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

55–70 phút, mức L4 / D4.
