# UNIT-ETAG-001 — Lost update trong catalog thư viện

## Mục tiêu
Tự tái hiện và điều tra xung đột cập nhật từ hai client độc lập.

## Bối cảnh thực tế
Hai thủ thư đồng thời sửa thông tin một cuốn sách. Cả hai màn hình báo lưu thành công, nhưng thay đổi của một người biến mất.

## Bạn cần làm gì
Chạy mô phỏng, thu thập request/response và phiên bản bản ghi, đưa ra hypothesis, sửa starter rồi xác minh.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell; không cần database hay cloud.

## Chạy nhanh
```powershell
dotnet run --project starter/LibraryCatalog.csproj -- reproduce
```

## Cách reproduce vấn đề
Từ thư mục unit chạy `./reproduce.ps1`. Quan sát hai lần lưu và trạng thái cuối cùng.

## Những gì cần quan sát
So sánh tiêu đề, số lượng bản sao, revision trước/sau hai lần ghi; ghi lại thao tác nào bị mất.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem [hint-01](hints/hint-01.md), [hint-02](hints/hint-02.md), [hint-03](hints/hint-03.md) theo thứ tự.

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md).

## Expected Results
Starter phải chứng minh mất cập nhật. Sau sửa, client thứ hai phải nhận conflict và không làm mất thay đổi của client thứ nhất.

## Estimated Time
40 phút.
