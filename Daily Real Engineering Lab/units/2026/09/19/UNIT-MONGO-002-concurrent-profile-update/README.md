# UNIT-MONGO-002 — Concurrent Profile Update Loses a Valid Change

## Mục tiêu
Điều tra một lỗi cập nhật đồng thời trong MongoDB khi hai request sửa các phần độc lập của cùng customer profile.

## Bối cảnh thực tế
Customer Profile API cho phép cập nhật địa chỉ giao hàng và tùy chọn liên lạc. Hai request gần như đồng thời đều trả về thành công, nhưng sau đó một thay đổi hợp lệ biến mất. Không có exception và log ứng dụng cho thấy cả hai request đã hoàn tất.

## Bạn cần làm gì
Chạy mô phỏng, thu thập evidence về trạng thái document qua từng bước, đưa ra hypothesis, sửa cơ chế cập nhật trong `starter/`, rồi chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell
- Lab dùng in-memory document store mô phỏng semantics cần điều tra, không yêu cầu MongoDB server.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy hai logical request trên cùng document theo một interleaving cố định. Script chỉ pass khi triệu chứng mất dữ liệu được tái hiện đúng.

## Những gì cần quan sát
- Snapshot mà mỗi request đọc ban đầu.
- Thứ tự hai request ghi dữ liệu.
- Final document sau khi cả hai request báo thành công.
- Có hay không một signal cho biết dữ liệu nền đã thay đổi kể từ lúc request đọc.

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
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Trước fix, cả hai operation có thể báo thành công nhưng final document thiếu một thay đổi. Sau fix, conflict phải được phát hiện hoặc hai thay đổi độc lập phải được bảo toàn theo contract đã chọn.

## Estimated Time
45 phút.