# UNIT-DI-001 — Bản kê phòng thi xuất lẫn dữ liệu giữa hai ca

## Mục tiêu

Tự reproduce lỗi giữa các lượt xuất bản kê trong ứng dụng .NET, phân tích bằng chứng, sửa code và xác minh cả hành vi lẫn cấu hình service graph.

## Bối cảnh thực tế

Worker của trung tâm khảo thí xuất danh sách chỗ ngồi cho ca sáng và ca chiều tuần tự trong cùng một tiến trình. Ca sáng chính xác; ca chiều lại chứa dòng của ca trước. Mỗi bản kê phải độc lập, không phụ thuộc thứ tự chạy.

## Bạn cần làm gì

1. Chạy starter, ghi số dòng và session ID của từng bản kê.
2. Nêu ít nhất hai giả thuyết có thể kiểm chứng.
3. Sửa **starter/Program.cs** (không sửa solution) để mỗi lượt xuất độc lập.
4. Chạy `verify.ps1`, giải thích vì sao phải kiểm tra thêm service graph.
5. So sánh với reference solution sau khi tự thử.

## Yêu cầu môi trường

.NET SDK 8.x, PowerShell 5.1+ hoặc 7+. Không cần Docker, Azure, database hoặc NuGet ngoài shared framework.

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

Hoặc:

```powershell
dotnet run --project ./starter/ExamManifestLab.csproj -- run
dotnet run --project ./starter/ExamManifestLab.csproj -- reproduce
```

## Cách reproduce vấn đề

`./reproduce.ps1` trả exit code 0 **khi chứng minh được lỗi của starter**, không có nghĩa là đã sửa. Sau khi chỉnh sửa `starter/Program.cs`, chạy `./verify.ps1`: script này phải thất bại trên starter ban đầu và chỉ thành công khi cả output lẫn cấu hình DI hợp lệ.

## Những gì cần quan sát

- Ca sáng mong đợi hai dòng `AM-EXAM`; ca chiều chỉ một dòng `PM-EXAM|B03`.
- Có dòng nào ngoài dữ liệu đầu vào ca chiều không? Vì sao ca đầu vẫn đúng?
- Chế độ `verify` kiểm tra thêm điều gì ngoài output?
- Xem [evidence/observations.md](evidence/observations.md) và ghi lại kết quả thực tế.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

Ghi tại [workspace/my-investigation.md](workspace/my-investigation.md).

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution

**Spoiler — chỉ xem sau khi tự điều tra:** [solution/README.md](solution/README.md).

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time

35–50 phút (L1 / D3).
