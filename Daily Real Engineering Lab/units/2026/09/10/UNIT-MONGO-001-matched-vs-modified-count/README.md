# UNIT-MONGO-001 — Update hợp lệ nhưng API báo document không tồn tại

## Mục tiêu

Điều tra semantics của MongoDB update result và phân biệt chính xác giữa document được tìm thấy với document thực sự thay đổi dữ liệu.

## Bối cảnh thực tế

Một Profile API cho phép người dùng cập nhật `DisplayName`. Khi gửi một giá trị mới, API trả `200 OK`. Nhưng nếu client retry cùng request với đúng giá trị hiện tại, API lại trả `404 Not Found` dù document vẫn tồn tại trong MongoDB.

Đây là một endpoint được thiết kế để có thể retry an toàn, nên hành vi trên tạo ra false failure và khiến client thực hiện retry không cần thiết.

## Bạn cần làm gì

1. Setup local MongoDB.
2. Chạy starter và reproduce symptom.
3. Ghi lại update result ở lần cập nhật thay đổi dữ liệu và lần retry cùng payload.
4. Đưa ra hypothesis về việc API đang dùng signal nào để quyết định `NotFound`.
5. Sửa code trong `starter/`.
6. Chạy `verify.ps1` để chứng minh cả update thật và idempotent no-op đều có semantics đúng.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- Docker Desktop hoặc Docker Engine có Docker Compose
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script phải chứng minh deterministic rằng:

- document tồn tại trước request
- lần update đầu thành công
- retry cùng payload bị starter phân loại thành `NOT_FOUND`

## Những gì cần quan sát

Tập trung vào các giá trị được log cho mỗi update:

- `MatchedCount`
- `ModifiedCount`
- trạng thái API mô phỏng trả về

Không giả định rằng một update không làm thay đổi bytes đồng nghĩa với việc query filter không match document.

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

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- Update từ `Ha` → `Ha Nguyen`: `OK`.
- Retry `Ha Nguyen` → `Ha Nguyen`: starter báo `NOT_FOUND`.
- Document vẫn tồn tại và vẫn có `DisplayName = "Ha Nguyen"`.

### Sau khi sửa

- Update có thay đổi dữ liệu: `OK`.
- Retry cùng payload trên document vẫn tồn tại: `OK`.
- Update với `_id` không tồn tại: `NOT_FOUND`.

## Troubleshooting

Nếu container chưa sẵn sàng, chạy:

```powershell
docker compose ps
docker compose logs mongo
```

Sau đó chạy lại `./setup.ps1`.

## Estimated Time

30–45 phút.
