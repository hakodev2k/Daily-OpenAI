# UNIT-CS-016 — Exception Filter Before Stack Unwind

## Mục tiêu

Điều tra thứ tự thực thi của exception handling trong C# khi có `catch when (...)` và `finally`, dựa trên evidence thay vì đoán từ thứ tự source code.

## Bối cảnh thực tế

Một worker giữ trạng thái “operation active” trong một scope có cleanup ở `finally`. Khi exception xảy ra, telemetry ghi nhận trạng thái bất ngờ tại thời điểm quyết định có bắt exception hay không, dù cleanup vẫn chạy trước khi method kết thúc.

## Bạn cần làm gì

Chạy starter, ghi lại timeline, đưa ra hypothesis về thứ tự runtime, sửa learner-editable code để policy không quan sát state chưa được cleanup, rồi verify.

## Yêu cầu môi trường

.NET SDK 8.x và PowerShell.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Thứ tự các marker `work`, `policy`, `cleanup`, `caught`.
- Giá trị `ActiveOperations` tại từng marker.
- Phân biệt thời điểm runtime đánh giá handler với thời điểm stack cleanup hoàn tất.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã tự điều tra.

[Reference Solution](solution/README.md)

## Expected Results

Starter tạo timeline cho thấy policy quan sát state khác với state sau cleanup. Bản sửa phải để quyết định/telemetry phụ thuộc vào state đã ổn định sau cleanup.

## Estimated Time

35–50 phút.
