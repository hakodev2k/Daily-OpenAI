# UNIT-HMACROT-001 — Quyết định xoay khóa webhook khi retry kéo dài hơn thời hạn thu hồi

## Mục tiêu
Đưa ra quyết định kỹ thuật có thể bảo vệ bằng bằng chứng khi hệ thống ASP.NET Core nhận webhook ký HMAC từ đối tác đang xoay khóa. Phân biệt **delivery**, **authentication**, **business processing** và **recovery**; lập ADR, thử kịch bản và xác định điều kiện GO/NO-GO.

## Bối cảnh thực tế
Dịch vụ thông quan kiện hàng nhận sự kiện `ParcelReleased` từ đối tác. Sau đợt xoay khóa, một số kiện hàng hợp lệ không được giải phóng dù dashboard vẫn báo webhook 2xx phần lớn thời gian. Security và Operations đặt ra các yêu cầu có vẻ cùng hợp lý nhưng cần đối chiếu với hợp đồng retry thực tế.

## Bạn cần làm gì
1. Đọc [scenario](docs/scenario.md), [partner contract](evidence/partner-contract.md), [security policy](evidence/security-policy.md).
2. Dựa trên [delivery timeline](evidence/delivery-events.csv), lập ma trận event nào hợp lệ, event nào cần điều tra và tại sao.
3. So sánh ít nhất ba phương án; xác định hard constraints, assumptions và các điểm chưa có bằng chứng.
4. Hoàn thiện [ADR của bạn](workspace/decision-record.md), kế hoạch rollout/rollback, idempotency và kiểm thử.
5. Dùng [acceptance checklist](verification/acceptance-checklist.md) tự đánh giá trước khi xem lời giải.
6. Xử lý [requirement change](extensions/requirement-change.md).

## Yêu cầu môi trường
Chỉ cần trình đọc Markdown/CSV và PowerShell hoặc Excel tùy chọn. Không cần tài khoản Azure, GitHub secret hay hệ thống production. Toàn bộ dữ liệu synthetic.

## Chạy nhanh
Mở lần lượt `docs/scenario.md` → `evidence/delivery-events.csv` → `workspace/decision-record.md` → `verification/acceptance-checklist.md`.

## Cách reproduce vấn đề
Đây là **L5 Design Decision Lab**, không có starter application lỗi để chạy. Tái hiện mâu thuẫn bằng cách áp dụng các mốc thời gian và quy tắc hợp đồng vào từng hàng CSV. Tính chênh lệch giữa thời điểm phát sinh, lần thử đầu, lần retry và hạn thu hồi khóa; ghi quyết định cho mỗi event.

## Những gì cần quan sát
Theo dõi sự khác nhau giữa HTTP 2xx, tính hợp lệ của chữ ký, thời điểm event được xử lý đúng một lần và trạng thái kiện hàng. Đặc biệt xem các trường hợp **first successful delivery** xảy ra sau mốc thu hồi, không chỉ tỷ lệ thành công trung bình.

## Quy tắc làm lab
1. Reproduce từ dữ liệu trước.
2. Viết ít nhất ba hypotheses.
3. Đề xuất phương án và trade-offs.
4. Verify bằng checklist và event matrix.
5. Chỉ sau đó mới xem reference solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi hoàn thành ADR:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
75 phút — L5 / D4. Không có lệnh build/test vì đây là bài tabletop decision; kiểm chứng bằng event matrix và acceptance gates.
