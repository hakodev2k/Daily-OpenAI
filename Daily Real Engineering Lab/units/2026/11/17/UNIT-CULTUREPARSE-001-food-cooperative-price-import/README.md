# UNIT-CULTUREPARSE-001 — Giá nhập hàng thay đổi giữa các máy chủ

## Mục tiêu
Điều tra sai lệch dữ liệu số khi cùng một file đối tác được xử lý ở hai môi trường khác nhau; chứng minh nguyên nhân bằng reproduction và regression test.

## Bối cảnh thực tế
Hợp tác xã thực phẩm nhận bảng giá từ nhà cung cấp quốc tế. Worker tại staging và production chạy cùng source nhưng một môi trường báo lỗi nhập liệu, môi trường kia ghi nhận giá hợp lệ. Tác động: sai tổng tiền đối soát hoặc ngừng xử lý lô hàng.

## Bạn cần làm gì
Chạy starter trên các môi trường mô phỏng, ghi nhận kết quả, lập ít nhất hai hypothesis, sửa phần learner-editable và xác minh với bộ kiểm thử.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7; không cần database, cloud hay NuGet ngoài SDK.

## Chạy nhanh
`dotnet run --project starter/PriceImport.csproj -- --demo`

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1`. Script chạy các bản ghi giống nhau dưới hai locale, lưu ý giá trị và lỗi khác nhau.

## Những gì cần quan sát
So sánh kết quả parsing, tổng tiền, exception và locale hiện hành. Không kết luận từ tên môi trường mà chưa kiểm tra input, runtime configuration và execution path.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Chạy `pwsh ./verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints
[Progressive hints](hints.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution.md) — chỉ xem sau khi tự thử.

## Expected Results
Starter thể hiện kết quả khác nhau hoặc lỗi giữa các locale. Sau fix, cùng payload tạo cùng kết quả số chính xác; dữ liệu sai định dạng bị từ chối rõ ràng.

## Estimated Time
35–50 phút.
