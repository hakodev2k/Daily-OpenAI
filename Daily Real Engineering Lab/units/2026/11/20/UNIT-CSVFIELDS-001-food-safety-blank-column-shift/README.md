# UNIT-CSVFIELDS-001 — Dữ liệu kiểm nghiệm thực phẩm bị lệch cột khi nhập lô

## Mục tiêu
Điều tra một sai lệch dữ liệu âm thầm trong pipeline nhập kết quả kiểm nghiệm; chứng minh lỗi bằng fixture và thiết kế regression test bảo vệ tính toàn vẹn dữ liệu.

## Bối cảnh thực tế
Hệ thống .NET nhận file phân tách bằng dấu phẩy từ các trạm kiểm nghiệm. Một số mẫu chưa có giá trị đo; dashboard vẫn hiển thị kết quả nhưng số liệu của một số cột không khớp phiếu gốc. Đội vận hành cần ngăn ghi sai dữ liệu mà không làm hỏng các bản ghi hợp lệ.

## Bạn cần làm gì
Chạy starter, ghi nhận output và bằng chứng, lập ít nhất hai hypothesis, sửa code trong `starter/`, rồi chạy kiểm chứng độc lập. Không xem solution trước.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell. Không cần database, cloud hay NuGet bên ngoài.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy lệnh trên từ thư mục unit. So sánh từng dòng input với kết quả import. Script trả exit code 0 khi phát hiện đúng triệu chứng đã biết.

## Những gì cần quan sát
Kiểm tra số lượng trường, thứ tự ánh xạ, các mẫu có giá trị trống, và trường hợp sai schema. Ghi lại kết quả trong `workspace/my-investigation.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Trước: một bản ghi có trường thiếu bị ánh xạ sai. Sau: vị trí trường được bảo toàn; dữ liệu sai schema bị từ chối rõ ràng. [Chi tiết](expected-results.md).

## Estimated Time
35–50 phút.
