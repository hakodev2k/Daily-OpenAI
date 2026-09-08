# UNIT-CS-004 — Rethrow sai làm mất stack trace gốc

## Mục tiêu
Điều tra một lỗi diagnostic trong C#: exception vẫn được propagate nhưng stack trace bị reset tại tầng wrapper, khiến production investigation chỉ nhìn thấy nơi rethrow thay vì nơi lỗi thực sự phát sinh.

## Bối cảnh
Một payment orchestration service bắt exception từ repository để log thêm context rồi ném lại. Sau khi refactor, Application Insights chỉ còn stack bắt đầu từ service layer; dòng lỗi thật trong repository biến mất.

## Nhiệm vụ
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypotheses về lý do stack trace bị mất.
3. So sánh hành vi của `throw ex;` và `throw;`.
4. Sửa `starter/Program.cs` để giữ nguyên stack trace gốc.
5. Chạy `verify.ps1`.
6. Sau đó mới xem solution.

## Chạy
```powershell
./run.ps1
```

## Reproduce
```powershell
./reproduce.ps1
```

## Điều cần quan sát
- Exception type và message không đổi.
- Stack trace trước fix không còn frame `PaymentRepository.LoadAsync`.
- Sau fix, frame nguồn lỗi phải xuất hiện lại.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference solution
`solution/README.md`

## Expected result
**Before:** stack trace bị reset tại nơi rethrow.

**After:** stack trace giữ được frame nơi exception phát sinh.

## Estimated time
25–40 phút.
