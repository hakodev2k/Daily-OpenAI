# UNIT-ES-001 — Search Visibility After a Successful Write

## Mục tiêu
Điều tra vì sao một ticket vừa được tạo thành công nhưng request tìm kiếm ngay sau đó đôi lúc chưa thấy ticket.

## Bối cảnh thực tế
Support portal ghi ticket vào search index để agent có thể tìm theo mã. API create trả `201 Created`, nhưng automation tiếp tục gọi search ngay lập tức và thỉnh thoảng nhận zero results. Vài giây sau cùng query lại thấy dữ liệu.

## Bạn cần làm gì
1. Chạy starter.
2. Reproduce symptom.
3. Ghi ít nhất hai hypothesis dựa trên timeline.
4. Sửa learner-editable code trong `starter/` sao cho contract của workflow được đáp ứng mà không biến mọi write thành thao tác đồng bộ đắt đỏ một cách mù quáng.
5. Chạy `verify.ps1`.
6. Chỉ sau đó xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần Elasticsearch server; lab dùng deterministic simulator cho visibility semantics.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Thời điểm write được acknowledge.
- Thời điểm search bắt đầu trả document.
- Sự khác nhau giữa lookup theo workflow state và search visibility.
- Hành vi khi có nhiều write liên tiếp.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: create thành công nhưng immediate search có thể chưa thấy document.

After: workflow cần immediate visibility có contract rõ ràng và verify pass; implementation không dựa vào arbitrary sleep.

## Estimated Time
35 phút.