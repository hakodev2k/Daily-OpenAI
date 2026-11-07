# UNIT-LISTMUT-001 — Lô hàng không đủ điều kiện vẫn được đưa vào lịch lấy hàng

## Mục tiêu

Tự reproduce một lỗi xử lý batch trong C#; thu thập bằng chứng từ dữ liệu đầu vào/đầu ra, xác định điều kiện khiến kết quả sai, sửa mà vẫn giữ đúng thứ tự các lô hợp lệ và kiểm chứng bằng các trường hợp biên.

## Bối cảnh thực tế

Hệ thống điều phối của một food bank nhận danh sách các lô thực phẩm được tài trợ. Trước khi giao danh sách cho tài xế, ứng dụng phải loại các lô đã quá hạn hoặc đang bị cách ly kiểm tra. Một số batch chạy đúng, nhưng khi nhiều lô không hợp lệ nằm cạnh nhau, tài xế vẫn nhận lệnh đến lấy hàng không thể vận chuyển. Báo cáo vận hành cho thấy số lô được lên lịch cao hơn số lô đủ điều kiện.

## Bạn cần làm gì

1. Chạy bản starter và lưu lại các mã lô xuất hiện không đúng.
2. Viết ít nhất hai giả thuyết về nguyên nhân.
3. Xác định trường hợp đầu vào nhỏ nhất làm lỗi xuất hiện.
4. Chỉnh **chỉ** `starter/DonationDispatch.cs`, không sửa `checks/`.
5. Chạy verification; giải thích tính đúng đắn, thứ tự đầu ra và chi phí xử lý.

## Yêu cầu môi trường

.NET SDK 8 trở lên (có thể dùng SDK mới hơn theo `global.json`), PowerShell 5.1 hoặc 7; không cần database, Docker, Azure hay NuGet bên thứ ba.

## Chạy nhanh

Tại thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi tự sửa:

```powershell
./verify.ps1
```

Hoặc dùng `powershell -File .\reproduce.ps1` nếu không dùng PowerShell 7. Có thể dùng `dotnet run --project checks/Checks.csproj -- reproduce` và `-- verify` trên Linux/macOS.

## Cách reproduce vấn đề

`./reproduce.ps1` dựng một batch có hai lô không đủ điều kiện đứng cạnh nhau và một lô hợp lệ; script chỉ thành công khi quan sát được triệu chứng dự kiến trên starter. Lưu danh sách ID trước/sau và đối chiếu với chính sách eligibility trong `docs/scenario.md`.

## Những gì cần quan sát

- Danh sách ID trước và sau một lần chuẩn bị batch.
- Số lượng lô không đủ điều kiện còn sót.
- Khác biệt giữa trường hợp các lô không hợp lệ đứng liền nhau và đứng xen kẽ.
- Thứ tự tương đối của các lô hợp lệ.
- Kết quả lần chạy thứ hai trên cùng danh sách.

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

**Spoiler — chỉ đọc sau khi đã tự reproduce và thử sửa:** [Reference Solution](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

30–40 phút. Ghi bằng chứng vào [workspace/my-investigation.md](workspace/my-investigation.md).
