# UNIT-MONGO-002 — Update thành công nhưng API trả state cũ

## Mục tiêu

Điều tra một MongoDB update path nơi database đã thay đổi đúng nhưng response ngay sau update vẫn chứa state cũ. Bạn cần phân biệt write result, returned document và persisted document, sau đó sửa contract mà không thêm một round-trip đọc lại không cần thiết.

## Bối cảnh thực tế

Một API quản lý fulfillment chuyển shipment từ `Pending` sang `Ready`. Dashboard đôi khi hiển thị `Pending` ngay sau khi operator bấm chuyển trạng thái, dù refresh trang thì thấy `Ready`. Log cho biết update không ném exception và document trong MongoDB đã thay đổi.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` và ghi lại ba evidence: state trước update, document được method trả về, state thực sự trong database sau update.
2. Ghi ít nhất hai hypothesis vào `workspace/my-investigation.md`.
3. Sửa code trong `starter/` để method trả đúng post-update state mà vẫn chỉ cần một atomic update operation.
4. Chạy `verify.ps1`.
5. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- Docker Desktop hoặc Docker Engine có `docker compose`
- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./setup.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` reset dữ liệu, chạy starter implementation và chỉ PASS khi quan sát được symptom ban đầu: update persisted thành công nhưng object mà application nhận ngay từ operation chưa phản ánh state mới.

## Những gì cần quan sát

- `BEFORE_STATUS`
- `RETURNED_STATUS`
- `STORED_STATUS`
- số database operation trong update path

README không chỉ ra option/API nào cần đổi. Hãy tập trung vào contract của operation và evidence thực tế.

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

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- persisted document có `Status = Ready`
- object trả về từ update path vẫn có `Status = Pending`
- không có write exception

### Sau khi sửa

- persisted document và returned document đều có `Status = Ready`
- update vẫn là một atomic find-and-update operation
- `verify.ps1` exit code `0`

Nếu không kết nối được MongoDB, chạy `docker compose ps` và `docker compose logs mongo`.

## Estimated Time

25–40 phút.