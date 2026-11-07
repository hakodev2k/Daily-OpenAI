# UNIT-EF-005 — Filtered Include Tracking Surprise

## Mục tiêu

Điều tra một API đọc dữ liệu đơn hàng trả về nhiều child rows hơn điều kiện truy vấn yêu cầu, dù SQL-like intent của developer nhìn có vẻ đúng. Mục tiêu là phân biệt dữ liệu từ query với dữ liệu đang được giữ trong DbContext và xác minh một fix không làm sai business contract.

## Bối cảnh thực tế

Một endpoint quản trị tải trước một Order để kiểm tra lịch sử, sau đó trong cùng request cần lấy lại Order chỉ kèm các OrderLine đang ở trạng thái `Open`. Log cho thấy truy vấn thứ hai có filter, nhưng response cuối vẫn chứa cả line đã `Closed`.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại evidence và ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
3. Sửa trực tiếp code trong `starter/` để response chỉ chứa các line `Open` theo contract.
4. Giữ nguyên bước preload đầu tiên để mô phỏng request thực tế.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Internet chỉ cần cho lần NuGet restore đầu tiên

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Dữ liệu seed có 3 OrderLine, trong đó chỉ 2 line là `Open`.
- Bước query cuối có điều kiện lọc child collection.
- Starter vẫn in `returnedLines=3`.
- Không có exception và dữ liệu database không bị thay đổi.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- `openInStore=2`
- `returnedLines=3`
- `reproduce.ps1` pass khi symptom tồn tại

After:
- `openInStore=2`
- `returnedLines=2`
- tất cả line trả về đều có `Status=Open`
- `verify.ps1` pass trên code learner chỉnh trong `starter/`

Nếu không reproduce được, chạy `dotnet clean`, xóa `bin/obj`, rồi chạy lại `dotnet restore` trước khi thử lại.

## Estimated Time

35–50 phút.
