# UNIT-DOTNET-011 — Inline Task Continuation Stall

## Mục tiêu

Điều tra một background dispatcher bị đứng ngay khi báo hiệu một công việc đã hoàn tất, dù CPU thấp và không có exception.

## Bối cảnh thực tế

Một order dispatcher dùng một completion signal để báo cho phần quan sát biết khi job đã sẵn sàng. Ở môi trường test đơn giản mọi thứ trông bình thường, nhưng một execution path có thể khiến dispatcher giữ tài nguyên giới hạn rồi đứng vô thời hạn tại bước hoàn tất signal.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi lại thứ tự log và hypothesis.
3. Xác định vì sao producer không thể rời critical section.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để chứng minh flow kết thúc và behavior không bị bỏ qua.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0+
- PowerShell
- Không cần Docker hay external service

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ chạy starter với timeout ngắn và xác nhận process bị kẹt sau khi producer bắt đầu hoàn tất signal nhưng trước khi toàn bộ flow kết thúc.

## Những gì cần quan sát

- Thread nào đang giữ tài nguyên giới hạn.
- Log cuối cùng xuất hiện trước khi process đứng.
- Continuation có cần chạy ngay trên thread đang hoàn tất signal hay không.
- Producer và consumer có vô tình phụ thuộc vòng tròn vào cùng một synchronization primitive hay không.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Starter in ra các bước đầu của dispatcher.
- Process không kết thúc trong timeout.
- CPU không cần tăng cao.

### After

- Producer hoàn tất signal và rời critical section.
- Observer tiếp tục xử lý.
- Process in `completed` và exit code bằng 0.

Nếu không reproduce được, kiểm tra đang dùng đúng `starter/InlineContinuationLab.csproj` và không sửa starter từ lần chạy trước.

## Estimated Time

30–45 phút.