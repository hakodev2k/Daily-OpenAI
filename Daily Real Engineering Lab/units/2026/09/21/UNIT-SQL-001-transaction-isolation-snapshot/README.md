# UNIT-SQL-001 — Báo cáo tổng tiền không nhất quán trong một transaction

## Mục tiêu

Điều tra một lỗi consistency trong SQL Server bằng cách reproduce, thu thập evidence và xác định transaction boundary phù hợp cho một báo cáo tài chính.

## Bối cảnh thực tế

Một scheduled job tạo báo cáo settlement gồm hai số liệu: số payment đã `Captured` và tổng số tiền tương ứng. Cả hai câu query đều chạy trong cùng một transaction. Thi thoảng production ghi nhận hai số liệu không khớp với nhau dù không có exception và từng câu query riêng lẻ đều trả kết quả hợp lệ.

## Bạn cần làm gì

Chạy lab với hai session SQL đồng thời, ghi lại kết quả của từng statement, hình thành ít nhất hai hypothesis, sau đó sửa `starter/session-a-report.sql` để hai số liệu đại diện cho cùng một logical view của dữ liệu mà vẫn giữ writer hoạt động bình thường.

## Yêu cầu môi trường

- SQL Server 2022 Developer/Express hoặc tương thích
- `sqlcmd` có trong `PATH`
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy report session và writer session theo timeline deterministic. Không chỉnh `starter/` trước lần reproduce đầu tiên.

## Những gì cần quan sát

- Hai giá trị report được đọc tại hai thời điểm khác nhau trong cùng transaction.
- Writer hoàn thành thành công trong lúc report đang chạy.
- Không có statement nào báo lỗi nhưng invariant của report bị phá vỡ.

Ghi evidence và hypothesis vào `workspace/my-investigation.md` trước khi xem hints.

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

[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results

Trước fix, script phải chứng minh report có thể tạo một cặp số liệu không cùng logical view. Sau fix, `verify.ps1` phải xác nhận invariant được giữ và writer vẫn hoàn thành.

Nếu không reproduce được, kiểm tra SQL Server instance trong các script và bảo đảm tài khoản hiện tại có quyền tạo database.

## Estimated Time

50 phút