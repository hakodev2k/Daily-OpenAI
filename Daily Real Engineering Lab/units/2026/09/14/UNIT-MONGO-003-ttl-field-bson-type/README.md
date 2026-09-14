# UNIT-MONGO-003 — TTL Field BSON Type

## Mục tiêu

Điều tra vì sao một MongoDB TTL index tồn tại đúng tên và đúng field nhưng session document cũ vẫn không được dọn dẹp như kỳ vọng.

## Bối cảnh thực tế

Một service lưu session tạm trong MongoDB. Team đã tạo TTL index trên `ExpiresAt` và dashboard cho thấy document hết hạn vẫn tiếp tục tăng sau nhiều giờ. Application không báo lỗi khi ghi dữ liệu.

## Bạn cần làm gì

- Chạy starter và reproduce triệu chứng.
- Quan sát BSON type thực tế của `ExpiresAt`.
- Ghi hypothesis trước khi sửa.
- Sửa model/serialization trong `starter/` để field phù hợp với TTL semantics.
- Chạy `verify.ps1` để xác nhận contract dữ liệu.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần MongoDB server cho core lab.

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- `ExpiresAt` nhìn giống timestamp hợp lệ khi in ra.
- BSON type của field không phải kiểu mà TTL index cần để đánh giá expiration.
- Không có exception bắt buộc phải xuất hiện khi serialize document.

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

> Spoiler: chỉ xem sau khi đã thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

**Before**
- Reproduction xác nhận `ExpiresAt` được serialize thành BSON type không phù hợp cho TTL expiration.

**After**
- Verification xác nhận `ExpiresAt` được serialize thành BSON DateTime.
- Giá trị expiration vẫn biểu diễn đúng thời điểm UTC mong muốn.

## Estimated Time

25–40 phút.
