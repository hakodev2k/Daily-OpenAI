# UNIT-CS-002 — Recipient đã có trong set nhưng vẫn bị thêm lần nữa

## Mục tiêu

Điều tra một lỗi C# collections nơi cùng một logical recipient có thể xuất hiện hai lần trong tập hợp dùng để chống gửi trùng, dù code đã gọi `Contains` trước khi `Add`.

## Bối cảnh thực tế

Một notification batch service gom danh sách recipient vào `HashSet` để tránh gửi cùng một campaign hai lần. Ở một số batch, log cho thấy cùng recipient được enqueue hai lần sau khi dữ liệu profile được chuẩn hóa trong memory. Không có exception và lỗi xảy ra hoàn toàn deterministic với sample data của lab.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa.
3. Quan sát kết quả `Contains`, số phần tử trong set và dữ liệu của từng phần tử.
4. Xác định invariant nào của collection đang bị phá vỡ.
5. Sửa code trong `starter/` mà không loại bỏ bước deduplication.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script phải chứng minh starter tạo trạng thái không mong muốn thay vì chỉ chạy ứng dụng.

## Những gì cần quan sát

- Giá trị `ContainsBeforeSecondAdd`.
- Giá trị `SecondAddReturned`.
- `FinalCount` của collection.
- Hai item cuối cùng có đại diện cùng logical recipient hay không.

Không sửa code trước khi ghi hypothesis về điều kiện nào khiến một phần tử đã từng được thêm vào collection không còn được tìm thấy như mong đợi.

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

⚠️ Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference solution](solution/README.md)

## Expected Results

**Starter:** reproduction script phải xác nhận logical duplicate có thể được thêm vào set.

**Sau khi sửa:** `verify.ps1` phải xác nhận lookup vẫn ổn định sau bước normalize và `FinalCount=1`.

## Estimated Time

25–40 phút.
