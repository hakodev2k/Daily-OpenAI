# UNIT-TX-001 — Ca trực kiểm lâm được duyệt nghỉ đồng thời

## Mục tiêu

Điều tra một lỗi toàn vẹn nghiệp vụ xảy ra khi hai yêu cầu hợp lệ được xử lý gần như đồng thời; thiết kế và kiểm chứng một phương án giữ đúng quy tắc vận hành trong điều kiện concurrency.

## Bối cảnh thực tế

Trung tâm liên lạc của một vườn quốc gia có một ca trực với hai kiểm lâm viên. Quy định an toàn yêu cầu **ít nhất một người luôn trực**. Hai nhân viên gửi yêu cầu nghỉ cùng lúc. Giao diện trả về thành công cho cả hai, nhưng bảng trực sau đó không còn ai. Khi kiểm tra từng yêu cầu riêng lẻ, hệ thống thường xử lý đúng.

## Bạn cần làm gì

1. Chạy fixture để tái hiện sự cố; ghi lại số yêu cầu được duyệt và số người còn trực.
2. Đề xuất ít nhất hai hypothesis; xác định bằng chứng nào phân biệt chúng.
3. Sửa mã trong `starter/` để bảo vệ invariant, không làm mất khả năng duyệt các yêu cầu hợp lệ.
4. Chạy verification, phân tích trade-off giữa một tiến trình và nhiều instance.
5. Viết quyết định triển khai thực tế: vị trí thực thi invariant, cách xử lý conflict và quan sát vận hành.

## Yêu cầu môi trường

- .NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell 5.1.
- Không cần database, Docker, cloud hay NuGet bên ngoài.
- Fixture là mô hình cục bộ có điều khiển thứ tự chạy, **không phải một database engine**.

## Chạy nhanh

Tại thư mục unit:

```powershell
./setup.ps1
./reproduce.ps1
./run.ps1
```

Sau khi sửa mã trong `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`./reproduce.ps1` chạy 24 vòng độc lập với hai yêu cầu nghỉ cùng lúc. Script chỉ thành công khi chứng minh được triệu chứng của starter. Sau khi sửa, dùng `verify.ps1` thay vì xem reproduce thành công là tiêu chí chất lượng.

## Những gì cần quan sát

- Có bao nhiêu yêu cầu trả về approved?
- Ca trực còn bao nhiêu người?
- Điều gì khác nhau giữa trường hợp 1, 2 và 3 nhân viên?
- Trình tự xử lý có ảnh hưởng đến kết quả hay không?
- Một phương án sửa có còn đúng khi triển khai nhiều replica không?

Đọc [bằng chứng incident](evidence/incident.md), ghi nhận suy luận vào [workspace](workspace/my-investigation.md) trước khi xem đáp án.

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

> **Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi đã thử sửa và verify. Mã tham khảo nằm trong `solution/`.

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

60–75 phút. Đây là bài **L5 Design Decision / D4**, có fixture thực thi và bài tập đánh giá triển khai production.
