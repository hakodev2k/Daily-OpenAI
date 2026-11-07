# UNIT-COSMOS-001 — Phân vùng redemption voucher khi một hợp tác xã tạo tải đột biến

## Mục tiêu

Điều tra dữ liệu phân bố không đều và đề xuất partition strategy cho hệ thống đổi voucher trợ giá hạt giống, cân bằng **throughput, point-read, idempotency và chi phí truy vấn**.

## Bối cảnh thực tế

Một nền tảng trợ giá hạt giống cho phép nông dân đổi voucher qua máy quét tại các hợp tác xã. Trong ngày phát giống, nhiều yêu cầu cùng lúc đến từ một điểm phát lớn; các điểm nhỏ vẫn hoạt động bình thường. Hệ thống đang có nhiều phản hồi throttling và dashboard cập nhật chậm dù tổng ngân sách tài nguyên có vẻ còn dư.

## Bạn cần làm gì

1. Chạy mô phỏng chính sách hiện tại và ghi lại số lượng voucher theo bucket.
2. Đưa ra ít nhất ba giả thuyết và xác định các bằng chứng cần đo trong Cosmos DB thực.
3. Thay đổi **duy nhất** `starter/policy.json` để thử một partition strategy.
4. Chạy verify, sau đó giải thích tính đúng đắn của retry và cách đọc voucher bằng ID.
5. Viết một quyết định thiết kế ngắn trong `workspace/my-investigation.md`, bao gồm trade-off và kế hoạch triển khai.

## Yêu cầu môi trường

PowerShell 7 (`pwsh`), không cần Azure subscription, Docker hoặc NuGet. Mô phỏng dùng 1.000 sự kiện synthetic cố định, không gọi Cosmos DB.

## Chạy nhanh

Tại thư mục unit:

```powershell
pwsh -NoProfile -File ./run.ps1
pwsh -NoProfile -File ./reproduce.ps1
```

## Cách reproduce vấn đề

Chạy `reproduce.ps1` với starter nguyên bản. Lệnh này **thành công khi phát hiện đúng các acceptance checks đang không đạt**; không phải lệnh xác nhận hệ thống đã sửa.

## Những gì cần quan sát

Ghi lại `maxBucketShare`, `bucketCount`, `replayStable` và `pointReadFromVoucherId`. So sánh các chỉ số với mục tiêu trong `docs/scenario.md`. Chú ý sự khác nhau giữa số lượng logical bucket trong mô phỏng và phân bố RU/s trên physical partition của Cosmos DB.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `pwsh -NoProfile -File ./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference Solution — chỉ mở sau khi tự thử](solution/README.md)

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

65–80 phút (L5 Design Decision, D4).
