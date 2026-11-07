# UNIT-EF-003 — Query Works in Repository, Fails at Enumeration

## Mục tiêu

Điều tra một EF Core query được tạo thành công trong data-access layer nhưng chỉ thất bại khi caller bắt đầu enumerate kết quả. Mục tiêu là xác định boundary nào đang sở hữu query execution và sửa code để lifetime của data-access resource khớp với thời điểm query thực sự chạy.

## Bối cảnh thực tế

Một API nội bộ dùng repository để lấy danh sách sản phẩm sắp hết hàng. Code review nhìn khá sạch: repository tạo query, áp dụng filter/projection rồi trả kết quả cho service layer. Sau một refactor nhỏ nhằm “giữ query linh hoạt”, endpoint bắt đầu trả 500 ở runtime.

Điểm gây nhiễu là exception không xuất hiện ở nơi query được xây dựng mà xuất hiện muộn hơn tại caller.

## Bạn cần làm gì

1. Chạy `./reproduce.ps1`.
2. Xác định thời điểm database query thực sự được thực thi.
3. Ghi lại object/resource nào còn cần tồn tại tại thời điểm đó.
4. Sửa code trong `starter/` để caller vẫn nhận đúng 2 sản phẩm low-stock mà không phụ thuộc vào resource đã hết lifetime.
5. Chạy `./verify.ps1`.
6. Sau khi tự sửa, so sánh với `solution/README.md`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Internet ở lần restore NuGet đầu tiên

Không cần SQL Server, Docker hoặc Azure.

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ restore/build starter rồi chạy scenario. Reproduction hợp lệ khi process thất bại có kiểm soát và in marker:

```text
OBJECT_DISPOSED
```

## Những gì cần quan sát

- method repository hoàn thành mà chưa có exception
- exception xuất hiện khi caller bắt đầu lấy dữ liệu
- call site nơi query được enumerate
- lifetime của object chịu trách nhiệm query execution

README cố ý không chỉ ra API hoặc dòng code phải đổi.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ mở sau khi bạn đã reproduce và tự thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- repository trả về object query mà không throw
- lỗi chỉ xuất hiện khi caller consume dữ liệu
- process in `OBJECT_DISPOSED`

Sau khi sửa:

- `./verify.ps1` exit code 0
- output chứa `RESULT_COUNT=2`
- output chứa `SKU-LOW-001` và `SKU-LOW-002`
- không còn `OBJECT_DISPOSED`

## Estimated Time

30–50 phút.
