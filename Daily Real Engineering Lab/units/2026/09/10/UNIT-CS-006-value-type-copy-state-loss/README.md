# UNIT-CS-006 — Counter tăng trong log nhưng state lưu trữ không đổi

## Mục tiêu

Điều tra một lỗi C# semantics trong đó code có vẻ đã cập nhật state thành công ở từng bước, nhưng state đọc lại từ store vẫn giữ nguyên giá trị ban đầu.

## Bối cảnh thực tế

Một in-memory quota component giữ số lần gọi API theo tenant. Log trong request path cho thấy counter tăng, nhưng dashboard nội bộ luôn đọc lại `0`. Không có exception, không có race trong kịch bản reproduce, và cùng một key được sử dụng xuyên suốt.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại giá trị được quan sát bên trong mỗi lần update và giá trị đọc lại từ store.
3. Đưa ra ít nhất hai hypothesis trước khi sửa code.
4. Sửa code trong `starter/` để ba lần update được phản ánh đúng trong state lưu trữ.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay dịch vụ ngoài.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

`reproduce.ps1` xác nhận starter vẫn biểu hiện đúng symptom ban đầu; nó không phải script kiểm tra lời giải.

## Những gì cần quan sát

So sánh:

- `ObservedCount` ở từng iteration
- `PersistedCount` đọc lại sau khi update xong
- việc cùng một dictionary key vẫn tồn tại

Hãy tập trung vào đường đi của state giữa local variable và store thay vì chỉ nhìn output cuối cùng.

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

- Mỗi iteration in ra một counter đã tăng trong local processing.
- Giá trị đọc lại từ store sau cùng không phản ánh ba lần update.

### Sau khi sửa

- Ba update được lưu lại.
- `PersistedCount=3`.
- Không thay đổi key hoặc bỏ qua update nào.

## Estimated Time

25–40 phút.
