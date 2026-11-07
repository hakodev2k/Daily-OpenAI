# UNIT-SEMA-001 — EV Charger Command Admission

## Mục tiêu

Điều tra một lỗi điều phối bất đồng bộ trong .NET khi hệ thống phải bảo đảm **mỗi trạm sạc chỉ xử lý một lệnh điều khiển tại một thời điểm**. Tự thu thập bằng chứng, sửa đường đi của learner và kiểm tra cả correctness lẫn concurrency invariant.

## Bối cảnh thực tế

Một backend điều khiển trạm sạc EV nhận lệnh từ hệ thống vận hành. Lệnh thứ nhất đang giữ thiết bị để cập nhật firmware; lệnh thứ hai đến trong lúc thiết bị bận, với ngân sách chờ bằng 0. Theo hợp đồng, lệnh thứ hai phải trả về `Busy` và không được tác động đến thiết bị.

Thực tế, đôi khi cả hai lệnh cùng được thực hiện và lệnh đầu tiên báo lỗi sau khi hoàn thành công việc. Vấn đề có thể dẫn đến trạng thái thiết bị không nhất quán và khó phân biệt lỗi vận hành với lỗi hạ tầng.

## Bạn cần làm gì

1. Chạy starter và ghi lại kết quả hai lệnh, số tác vụ đồng thời tối đa và exception.
2. Đưa ra ít nhất hai giả thuyết trước khi đọc hints.
3. Chỉnh **chỉ** `starter/DispatchCoordinator.cs` để duy trì hợp đồng admission.
4. Chạy `verify.ps1`, kiểm tra thêm đường lỗi của callback và cancellation.
5. So sánh với reference solution, giải thích tại sao tăng capacity hoặc retry không phải cách sửa mặc định.

## Yêu cầu môi trường

- .NET SDK 10 (theo `global.json`), PowerShell 5.1+ hoặc PowerShell 7.
- Không cần Docker, database, Azure hay NuGet package bên ngoài.

## Chạy nhanh

Từ thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi tự sửa starter:

```powershell
./verify.ps1
```

Nếu không dùng PowerShell, chạy trực tiếp `dotnet run --project ./src/LabHarness/LabHarness.csproj -- reproduce` hoặc `-- verify`.

## Cách reproduce vấn đề

`reproduce.ps1` dựng hai lệnh theo thứ tự cố định: lệnh A đã bắt đầu nhưng chưa kết thúc; lệnh B thử vào ngay khi A đang chạy. Harness kiểm tra symptom có thực sự xuất hiện. Exit code 0 của **reproduce** có nghĩa là *lỗi đã được tái hiện*, không có nghĩa starter đúng.

## Những gì cần quan sát

- `SecondOutcome`: lệnh B được xử lý hay từ chối?
- `SecondWorkRan`: callback của B có thực sự chạy?
- `MaxConcurrentWork`: invariant một lệnh có bị vượt qua?
- `FirstError`: exception xảy ra ở giai đoạn nào?
- `ThirdOutcome`: coordinator có tiếp tục phục vụ sau sự cố không?

Không dựa vào timing của máy; harness sử dụng gate để điều khiển thứ tự sự kiện.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi đã tự điều tra và thử sửa.

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

55 phút (L2 investigation, D4). Ghi chú của bạn tại [workspace](workspace/my-investigation.md).
