# UNIT-PQ-001 — Xe vận chuyển máu khẩn vẫn bị điều phối sau

## Mục tiêu
Tái hiện lỗi thứ tự điều phối trong một bộ lập lịch .NET; thu thập bằng chứng, nêu ít nhất hai giả thuyết, sửa theo đúng hợp đồng nghiệp vụ và kiểm tra hồi quy.

## Bối cảnh thực tế
Trung tâm máu vùng điều phối xe vận chuyển túi máu bảo quản lạnh. Mỗi yêu cầu có mức khẩn cấp từ **0 (cao nhất)** đến **9 (thấp nhất)**. Một yêu cầu thường có thể được nâng mức khẩn sau khi đã tiếp nhận. Điều phối viên báo rằng giao diện hiển thị mức khẩn mới nhưng xe vẫn nhận yêu cầu khác trước, làm chậm ca cấp cứu.

## Bạn cần làm gì
1. Chạy chương trình kiểm tra trên bộ dữ liệu cố định, ghi lại case thất bại.
2. So sánh thứ tự mong đợi với thứ tự thực tế, xác định invariant bị vi phạm.
3. Sửa **starter/DispatchQueue.cs**, không sửa bộ kiểm tra trong **starter/Program.cs**.
4. Chạy verify và bảo vệ các trường hợp nâng mức khẩn nhiều lần, xử lý đúng một lần và FIFO khi đồng mức.

## Yêu cầu môi trường
- .NET SDK 8.x trên Windows, PowerShell 7 hoặc Windows PowerShell 5.1.
- Không cần Docker, database, Azure hay NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

Sau khi tự sửa `starter/DispatchQueue.cs`:

```powershell
./scripts/verify.ps1
```

Có thể chạy trực tiếp `dotnet run --project ./starter/DispatchLab.csproj` để xem từng case.

## Cách reproduce vấn đề
Giữ nguyên starter, chạy `./scripts/reproduce.ps1`. Script chỉ báo reproduce thành công khi harness **thực sự** phát hiện lỗi hợp đồng, không nhầm lỗi build thành lỗi nghiệp vụ. Xem thêm [scenario](docs/scenario.md).

## Những gì cần quan sát
- Case nào xuất ra `FAIL`, case nào `PASS`?
- Yêu cầu khẩn có được chọn đầu tiên không?
- Tổng số yêu cầu được điều phối có đúng và không lặp không?
- Nếu nhiều yêu cầu có cùng mức khẩn, thứ tự có ổn định không?

Ghi kết quả vào [workspace/my-investigation.md](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md). Chỉ mở lần lượt khi cần.

## Reference Solution
**Spoiler:** chỉ đọc [solution/README.md](solution/README.md) sau khi tự điều tra và verify.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md).

## Estimated Time
35–45 phút. D3 / L1 (deterministic failure, yêu cầu giải thích invariant và trade-off).
