# UNIT-JSON53-001 — Parcel event bị mất khi đồng bộ

## Mục tiêu
Điều tra sự mất dữ liệu trong pipeline tiếp nhận event từ đối tác, chứng minh bằng evidence và sửa tại ranh giới xử lý.

## Bối cảnh thực tế
Trung tâm vận chuyển nhận ba event quét kiện hàng có ID do hệ thống đối tác cấp. Một event bị xem là trùng dù đối tác xác nhận ba ID khác nhau. Log không có lỗi parse.

## Bạn cần làm gì
Chạy chương trình, ghi lại các khóa được nhận và bị từ chối; đưa ra ít nhất hai giả thuyết, sửa code trong `starter/`, rồi chạy kiểm chứng.

## Yêu cầu môi trường
.NET SDK 8 (xem `global.json`), PowerShell 7 hoặc Windows PowerShell. Không cần Docker, database hay API trả phí.

## Chạy nhanh
Tại thư mục unit: `pwsh -File scripts/setup.ps1` rồi `pwsh -File scripts/reproduce.ps1`.

## Cách reproduce vấn đề
Chạy `dotnet run --project starter/ParcelEvents.csproj`. Trạng thái ban đầu trả exit code 1. `scripts/reproduce.ps1` xác nhận đúng failure và trả exit code 0.

## Những gì cần quan sát
So sánh `ACCEPT`, `DUPLICATE`, số lượng event và dữ liệu JSON đầu vào; xác định có sự khác biệt giữa giá trị đầu vào và identity sau khi xử lý hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Chạy `pwsh -File scripts/verify.ps1` (phải pass sau khi sửa).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45 phút.
