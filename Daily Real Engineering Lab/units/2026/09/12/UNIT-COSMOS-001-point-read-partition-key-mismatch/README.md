# UNIT-COSMOS-001 — Point read trả 404 cho document chắc chắn tồn tại

## Mục tiêu

Điều tra một read path kiểu Azure Cosmos DB point read trả `404 NotFound` dù document có thật và có thể nhìn thấy trong dữ liệu seed.

## Bối cảnh thực tế

Support API đọc ticket theo `id` và tenant. Sau một thay đổi nhỏ ở mapping request, một số ticket cũ bắt đầu trả 404. Query dạng scan/list vẫn nhìn thấy ticket, nhưng point read theo `id` lại thất bại. Không có lỗi authentication và dữ liệu không bị xóa.

## Bạn cần làm gì

1. Chạy starter.
2. Reproduce case ticket tồn tại nhưng point read thất bại.
3. Ghi hypothesis vào `workspace/my-investigation.md`.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để chứng minh case hợp lệ đọc được và case không tồn tại vẫn trả 404.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần Azure subscription; lab dùng simulator local cho contract point-read + partition key.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy starter với một ticket đã được seed sẵn và xác nhận symptom `STATUS=404`.

## Những gì cần quan sát

- `id` request và `id` của item seed có trùng nhau không.
- Tenant metadata đi vào read path dưới dạng nào.
- Contract của point read khác gì với query/scan.
- Những giá trị nào thực sự tạo thành địa chỉ logic của một item.

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

> Spoiler: chỉ xem sau khi đã reproduce và tự thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Ticket `case-1042` tồn tại trong seed data.
- Read path trả `STATUS=404` cho request tenant hợp lệ.

### After

- Request hợp lệ trả `STATUS=200` và đúng ticket.
- Ticket không tồn tại vẫn trả `STATUS=404`.
- Fix giữ point read deterministic, không chuyển sang scan để che symptom.

## Estimated Time

30–45 phút.