# UNIT-MONGO-001 — MongoDB Concurrent Array Update Lost Change

## Mục tiêu
Điều tra một lỗi concurrency trong luồng cập nhật document MongoDB và chứng minh bản sửa giữ được cả tính đúng đắn lẫn hành vi idempotent.

## Bối cảnh thực tế
Một internal API quản lý thành viên của team. Hai request thêm hai user khác nhau gần như đồng thời. Cả hai request đều trả thành công, nhưng khi đọc lại team đôi lúc chỉ thấy một user mới.

## Bạn cần làm gì
1. Chạy starter để tái hiện triệu chứng.
2. Ghi hypothesis trước khi xem hints.
3. Xác định boundary nào cần đảm bảo tính nguyên tử.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra cả concurrent update và duplicate request.

## Yêu cầu môi trường
- .NET SDK 8.x
- Không cần MongoDB server thật; lab dùng in-memory document-store simulator để tái hiện semantics cần điều tra.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script phải chứng minh rằng hai operation đều báo thành công nhưng final state không luôn chứa đầy đủ hai thay đổi.

## Những gì cần quan sát
- Hai operation hoàn tất mà không throw exception.
- Final document có thể thiếu một membership change.
- Version của document và thứ tự read/write trong output.
- CPU, timeout hay retry không phải tín hiệu chính của bài này.

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
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
Khoảng 50 phút.