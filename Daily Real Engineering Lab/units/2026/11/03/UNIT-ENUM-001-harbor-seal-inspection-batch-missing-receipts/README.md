# UNIT-ENUM-001 — Harbor Gate Seal Scans: Batch Accepted, No Receipts

## Mục tiêu

Điều tra một batch import có số lượng đầu vào hợp lệ nhưng không tạo phiếu kiểm tra. Phân biệt symptom, evidence, giả thuyết và thay đổi có kiểm chứng.

## Bối cảnh thực tế

Một cổng cảng nhận dữ liệu kiểm tra niêm phong container từ adapter của thiết bị quét. Dashboard báo đã nhận ba scan; hệ thống lưu trữ không thấy phiếu nào. Dữ liệu test được cố định, không kết nối thiết bị hoặc dịch vụ thật.

## Bạn cần làm gì

1. Reproduce và ghi lại số scan được báo nhận, được lưu, bị từ chối và số scan còn trong nguồn.
2. Nêu ít nhất hai giả thuyết và tìm bằng chứng loại trừ.
3. Sửa logic trong `starter/ScanImportService.cs`, giữ nguyên business validation.
4. Chạy verification trên chính `starter/`, sau đó mới đối chiếu reference.

## Yêu cầu môi trường

.NET SDK 10.x và PowerShell 7 hoặc Windows PowerShell. Không cần NuGet package bên ngoài, Docker hay cloud. Kiểm tra: `dotnet --info`.

## Chạy nhanh

Từ thư mục unit:

```powershell
./setup.ps1
./reproduce.ps1
```

Hoặc `dotnet run --project starter/SealImportLab.csproj -- --reproduce`.

## Cách reproduce vấn đề

`./reproduce.ps1` kiểm tra triệu chứng của starter bằng nguồn dữ liệu một lượt; script thành công khi lỗi gốc tái hiện đúng. Đây **không phải** bài test chứng minh đã sửa xong.

## Những gì cần quan sát

So sánh `received`, `stored`, `rejected`, `source_remaining` và `enumerations` với [incident evidence](evidence/incident-log.md). Đối chiếu với dữ liệu đầu vào và behavior của nguồn replayable. Đừng kết luận từ một counter duy nhất.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ mở [reference solution](solution/README.md) sau khi tự điều tra và chạy `./verify.ps1`.

## Expected Results

Trước sửa: `received=3 stored=0 rejected=0`. Sau sửa: `received=3 stored=2 rejected=1`, source chỉ được duyệt một lượt. `./verify.ps1` phải **fail** với starter ban đầu và **pass** sau khi sửa đúng.

## Estimated Time

40 phút. Ghi bằng chứng và kết luận vào [workspace](workspace/my-investigation.md).
