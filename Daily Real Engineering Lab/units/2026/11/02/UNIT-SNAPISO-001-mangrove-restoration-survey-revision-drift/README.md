# UNIT-SNAPISO-001 — Lịch sử khảo sát rừng ngập mặn tự thay đổi sau khi công bố

## Mục tiêu

Điều tra dữ liệu lịch sử thay đổi bất thường và refactor một component C# để các phiên bản khảo sát độc lập, không đổi public API.

## Bối cảnh thực tế

Hệ thống phục hồi rừng ngập mặn lưu các báo cáo hiện trường theo phiên bản. Sau khi cán bộ bổ sung ghi chú vào báo cáo đang làm việc, bản lịch sử đã công bố tuần trước cũng thay đổi. Kiểm toán không còn biết dữ liệu nào thực sự được công bố.

## Bạn cần làm gì

Reproduce, ghi bốn triệu chứng, lập hypothesis, chỉ sửa `starter/SnapshotStore.cs`, verify tất cả regression contracts, rồi so sánh reference solution. Không sửa `harness/`.

## Yêu cầu môi trường

.NET SDK 8, PowerShell; không cần Docker, cloud, database hoặc NuGet package bên ngoài SDK.

## Chạy nhanh

```powershell
./scripts/setup.ps1
./scripts/run.ps1
```

## Cách reproduce vấn đề

```powershell
./scripts/reproduce.ps1
```

Script này trả exit code 0 khi **phát hiện đúng lỗi starter**, không phải khi fix đã thành công.

## Những gì cần quan sát

- Báo cáo cũ có giữ nguyên sau khi input được chỉnh sửa?
- Caller nhận lịch sử có thể tác động tới lần đọc sau?
- Hai lần công bố có tạo ra hai revision độc lập?
- Danh sách lồng nhau có thể bị sửa qua một read-only API?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./scripts/verify.ps1` — cả bốn contract phải PASS.
5. Chỉ sau đó xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Giải thích và code tham chiếu](solution/README.md).

## Expected Results

Trước: `reproduce.ps1` phát hiện 4 FAIL. Sau: `verify.ps1` PASS 4/4 và giữ nguyên số lượng, thứ tự revision.

Nếu không reproduce, kiểm tra `dotnet --info`, `global.json`, chạy `dotnet run --project harness/Contracts.csproj -- inspect`.

## Estimated Time

55 phút.
