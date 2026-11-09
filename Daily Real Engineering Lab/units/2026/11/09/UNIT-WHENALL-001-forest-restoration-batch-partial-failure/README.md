# UNIT-WHENALL-001 — Forest survey batch: partial failure

## Mục tiêu
Điều tra và refactor pipeline nhập dữ liệu song song để một điểm khảo sát không hợp lệ không làm mất dữ liệu hợp lệ của các điểm khác.

## Bối cảnh thực tế
Hệ thống phục hồi rừng nhập báo cáo cây sống sót từ ba khu vực. Một báo cáo có dữ liệu bất thường; dashboard không còn hiển thị kết quả của các khu vực hợp lệ. Hãy xác định phạm vi ảnh hưởng trước khi sửa.

## Bạn cần làm gì
Reproduce, ghi nhận số báo cáo đầu vào/đầu ra, phân loại lỗi, đề xuất contract xử lý từng khu vực, sửa trong `starter/Program.cs`, verify và đánh giá trade-off.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần cloud, Docker hay NuGet ngoài SDK.

## Chạy nhanh
```powershell
dotnet --version
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; script xác nhận starter không đáp ứng contract. `./run.ps1` chạy starter trực tiếp và trả exit code khác 0 khi kết quả thiếu hoặc sai.

## Những gì cần quan sát
Ghi lại số input, số output, các khu vực xuất hiện và biến mất, exception xuất hiện trong console, và những task nào đã hoàn tất. Phân biệt lỗi riêng lẻ với lỗi toàn batch.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Chạy `./verify.ps1` (phải exit 0).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ mở sau khi tự sửa.

## Expected Results
Trước: một bản ghi không hợp lệ dẫn đến thiếu báo cáo hợp lệ, `FAIL` và exit 1. Sau: đúng ba kết quả, hai thành công, một lỗi có thể truy vết, `PASS` và exit 0.

## Estimated Time
45 phút.
