# UNIT-PG-002 — Batch Import Cannot Recover After One Invalid Row

## Mục tiêu
Điều tra một batch import chạy trong PostgreSQL transaction: một record lỗi được xử lý như lỗi cục bộ, nhưng các record hợp lệ phía sau vẫn không thể được lưu.

## Bối cảnh thực tế
Một worker nhập dữ liệu đối tác theo batch. Business cho phép bỏ qua record vi phạm constraint và tiếp tục xử lý các record còn lại. Log cho thấy exception của record lỗi đã được catch, nhưng batch cuối cùng vẫn thất bại và không commit được các record hợp lệ phía sau.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypothesis trước khi sửa.
3. Xác định transaction state thay đổi như thế nào sau statement thất bại.
4. Sửa `starter/Program.cs` để một record lỗi có thể được cô lập mà transaction chính vẫn tiếp tục hợp lệ.
5. Chạy `verify.ps1` trên code bạn sửa.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+
- Lab dùng simulator local, không cần PostgreSQL server.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy một batch cố định gồm record hợp lệ, một record vi phạm constraint, rồi thêm record hợp lệ. Starter mô phỏng PostgreSQL transaction semantics để symptom deterministic.

## Những gì cần quan sát
- statement nào thất bại
- trạng thái transaction sau exception
- record phía sau có được thực thi không
- transaction có commit được không

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
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** exception cục bộ được catch nhưng transaction không thể tiếp tục/commit batch như business mong muốn.

**After:** record lỗi bị bỏ qua có chủ đích, các record hợp lệ trước/sau vẫn được commit, và failure boundary được thể hiện rõ trong code.

## Estimated Time
35–55 phút.