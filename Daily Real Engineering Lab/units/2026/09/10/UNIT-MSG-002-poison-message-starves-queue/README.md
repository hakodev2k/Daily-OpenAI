# UNIT-MSG-002 — Một message lỗi khiến queue gần như không tiến triển

## Mục tiêu

Điều tra một failure mode trong message consumer khi một message xử lý thất bại lặp lại và các message hợp lệ phía sau không được xử lý như kỳ vọng.

## Bối cảnh thực tế

Một notification worker xử lý message theo thứ tự từ queue. Sau khi deploy một loại notification mới, dashboard cho thấy worker vẫn chạy và liên tục log lỗi, nhưng số message hoàn thành gần như đứng yên. Các message hợp lệ được enqueue sau đó cũng không tới được handler trong khoảng thời gian quan sát.

Lab dùng một queue simulator local, deterministic; không cần Azure Service Bus hoặc broker thật.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` để chứng minh symptom của starter.
2. Ghi evidence vào `workspace/my-investigation.md`.
3. Đưa ra ít nhất hai hypothesis về lý do queue không tiến triển.
4. Sửa code trong `starter/` để một message lỗi không thể giữ toàn bộ queue vô thời hạn.
5. Đảm bảo message lỗi vẫn được xử lý theo policy rõ ràng thay vì silently mất dữ liệu.
6. Chạy `verify.ps1` trên chính code bạn đã sửa.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Docker, database hay cloud account.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ thất bại nếu starter không còn thể hiện đúng symptom ban đầu.

## Những gì cần quan sát

Thu thập evidence từ output:

- message ID nào xuất hiện lặp lại
- delivery attempt tăng như thế nào
- số message được xử lý thành công
- số message còn pending
- dead-letter count

Không kết luận chỉ từ một exception. Hãy xem queue có tiến triển qua các message khác hay không.

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

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

### Starter

- cùng một message lỗi được nhận lại nhiều lần
- các message hợp lệ phía sau không được xử lý trong cửa sổ chạy
- queue vẫn còn pending message

### Sau khi sửa

- message lỗi chỉ được retry trong một giới hạn hữu hạn
- sau khi vượt policy đó, nó được chuyển sang terminal failure path có thể quan sát
- các message hợp lệ phía sau vẫn được xử lý
- queue không còn pending message trong scenario của lab

Nếu không reproduce được, kiểm tra bạn đang chạy đúng project trong `starter/` và chưa sửa starter trước khi chạy `reproduce.ps1`.

## Estimated Time

30–60 phút.
