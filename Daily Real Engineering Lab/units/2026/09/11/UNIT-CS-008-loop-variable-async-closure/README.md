# UNIT-CS-008 — Async Fan-out Uses the Wrong Tenant

## Mục tiêu

Điều tra một lỗi C# xuất hiện khi một batch tạo nhiều asynchronous work item trong vòng lặp. Bạn cần reproduce symptom, xác định lifetime/state nào đang được dùng khi delegate thực sự chạy, sửa implementation và verify rằng mỗi work item xử lý đúng tenant.

## Bối cảnh thực tế

Một scheduled export job tạo một task cho mỗi tenant. Ở production, batch nhỏ đôi khi báo `IndexOutOfRangeException`; một biến thể khác của cùng kiểu code có thể xử lý nhầm tenant. Review nhanh không thấy shared collection bị mutate và toàn bộ task đều được `await`.

Lab dùng một gate để làm failure deterministic: tất cả work item chỉ bắt đầu đọc dữ liệu sau khi vòng lặp đã kết thúc.

## Bạn cần làm gì

1. Chạy reproduction và ghi lại exception/symptom.
2. Ghi ít nhất hai hypothesis vào `workspace/my-investigation.md`.
3. Xác định giá trị nào được đọc lúc tạo work item và giá trị nào được đọc lúc delegate chạy.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để chứng minh ba tenant được xử lý đúng, mỗi tenant đúng một lần.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần database, Docker hay cloud account.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-CS-008-loop-variable-async-closure"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Expected starter symptom:

- batch không hoàn thành thành công;
- exception liên quan tới việc truy cập tenant theo index xuất hiện sau khi gate được mở;
- failure không phụ thuộc timing ngẫu nhiên.

## Những gì cần quan sát

- Có bao nhiêu work item được tạo?
- Delegate dùng dữ liệu nào từ scope bên ngoài?
- Giá trị đó được evaluate lúc delegate được tạo hay lúc delegate chạy?
- Tại thời điểm gate mở, vòng lặp đang ở trạng thái nào?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- `reproduce.ps1` xác nhận starter biểu hiện failure dự kiến.
- Batch không tạo được kết quả hợp lệ cho đủ ba tenant.

### Sau khi sửa

- `verify.ps1` exit code `0`.
- Output chứa đúng `alpha`, `bravo`, `charlie`.
- Không tenant nào bị xử lý lặp hoặc bị bỏ sót.
- Không serialize toàn bộ batch chỉ để né lỗi.

## Estimated Time

25–40 phút.