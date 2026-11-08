# UNIT-KEYROT-001 — Thiết kế rotation signing key cho hệ thống chi trả trợ cấp

## Mục tiêu
Thiết kế và kiểm chứng quy trình thay khóa ký JWT giữa API cấp token và các đối tác xác thực độc lập; cân bằng availability, security và khả năng rollback.

## Bối cảnh thực tế
Cổng trợ cấp thành phố phát hành JWT ký RS256 cho ba đối tác thanh toán. Trong đợt rotation dự kiến, một số giao dịch hợp lệ bị từ chối dù API phát hành vẫn hoạt động. Đối tác A cache JWKS 15 phút, B 60 phút, C chỉ đồng bộ mỗi 2 giờ. Token sống tối đa 45 phút. Mỗi đối tác có đồng hồ lệch tối đa 2 phút.

## Bạn cần làm gì
1. Đọc [timeline](evidence/timeline.csv) và [hợp đồng hệ thống](docs/scenario.md).
2. Dựng ma trận thời gian: lúc nào từng verifier có thể biết khóa mới, lúc nào token ký khóa cũ hết hạn.
3. Viết runbook rotation bình thường, rollback và emergency compromise; xác định tiêu chí go/no-go.
4. Phân biệt biện pháp ngăn token hợp lệ bị từ chối với biện pháp xử lý khóa bị lộ.
5. Đối chiếu [acceptance checks](docs/acceptance.md), sau đó mới mở lời giải.

## Yêu cầu môi trường
Không cần Azure, tài khoản trả phí hoặc công cụ ngoài. Spreadsheet hoặc Markdown đủ để chạy tabletop.

## Chạy nhanh
Mở `evidence/timeline.csv`; ghi quyết định vào `workspace/decision.md`. Mô phỏng các mốc 09:00, 09:15, 09:45, 10:00, 11:00, 12:00.

## Cách reproduce vấn đề
Tại 09:00 đổi khóa ký, cho verifier đọc theo lịch trong timeline. Với mỗi token, so sánh `kid` và bộ khóa mà verifier đang có tại thời điểm xác thực. Ghi tỷ lệ token hợp lệ bị từ chối theo đối tác; không giả định cache tự refresh ngay khi gặp `kid` lạ.

## Những gì cần quan sát
Phân biệt lỗi `unknown_kid`, `expired`, `invalid_signature`; xác định khoảng trống giữa publish, activate, retire và refresh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử thiết kế.
4. Verify qua ma trận.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án tham khảo](solution/README.md).

## Expected Results
Không từ chối token hợp lệ trong planned rotation; verifier không chấp nhận chữ ký sai; quy trình compromise có quyết định rõ ràng về đánh đổi availability/security.

## Estimated Time
55–70 phút.
