# UNIT-CTX-001 — Audit thiếu correlation ID sau khi chuyển việc sang ThreadPool

## Mục tiêu
Điều tra vì sao hai yêu cầu độc lập đều hoàn thành nhưng audit receipt không giữ được correlation ID của yêu cầu tương ứng; đề xuất cách truyền ngữ cảnh bền vững qua ranh giới xử lý bất đồng bộ.

## Bối cảnh thực tế
Dịch vụ Fraud Risk ghi audit event sau khi chấm điểm giao dịch. Dashboard hiển thị đầy đủ event ID, nhưng truy vấn theo correlation ID không tìm thấy bản ghi. Một đường xử lý async khác vẫn trả đúng ID. Triệu chứng xuất hiện ngay cả khi chỉ có hai yêu cầu đồng thời.

## Bạn cần làm gì
1. Reproduce và ghi lại bảng `expected / control / audit` cho CASE-A, CASE-B.
2. Đưa ra ít nhất ba hypothesis và bằng chứng loại trừ.
3. Chỉnh sửa **starter/** để cả hai audit receipt khớp yêu cầu gốc, kể cả khi chạy song song.
4. Chạy verify, giải thích vì sao sửa đổi vẫn đúng khi hàng đợi chuyển sang worker/process khác.

## Yêu cầu môi trường
- .NET SDK 8 (xem `global.json`); Windows PowerShell hoặc PowerShell 7.
- Không cần Azure, SQL, Docker, NuGet bên ngoài hay secret.

## Chạy nhanh
Từ thư mục unit:

```powershell
./setup.ps1
./reproduce.ps1
./run.ps1
```

Hoặc: `dotnet run --project ./starter/CorrelationLab.csproj -- --inspect`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter chưa chỉnh sửa. Script thành công khi quan sát được **triệu chứng có chủ đích**; đây chưa phải là verification. Ghi lại hai dòng output và thử chạy lại để kiểm tra tính lặp lại.

## Những gì cần quan sát
- `control` có khớp `expected` ở từng yêu cầu không?
- `audit` có khớp `expected` không?
- Kết quả có phụ thuộc vào thứ tự hoàn thành của hai yêu cầu không?
- `verify.ps1` báo gì trước và sau khi bạn sửa?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự thử:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time
55 phút.
