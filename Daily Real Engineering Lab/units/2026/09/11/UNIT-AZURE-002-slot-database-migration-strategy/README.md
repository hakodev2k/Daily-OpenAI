# UNIT-AZURE-002 — Deployment Slot & Database Migration Strategy

## Mục tiêu

Ra quyết định rollout cho một ASP.NET Core application trên Azure App Service khi application code có thể deploy bằng slot swap nhưng database schema là tài nguyên dùng chung và không thể rollback tức thời theo cùng cơ chế.

## Bối cảnh thực tế

Một CMS-backed content API đang chạy trên Azure App Service với production slot và staging slot. Team muốn zero-downtime release bằng slot swap. Release sắp tới thay đổi database schema và code mới bắt đầu ghi dữ liệu theo schema mới ngay sau khi traffic chuyển sang production.

Hệ thống có các ràng buộc:

- khoảng 150 request/second giờ cao điểm
- SLA 99.9%
- rollback application phải thực hiện được trong vài phút
- database dùng chung giữa staging và production
- team 5 backend developers, không có DBA trực 24/7
- downtime bảo trì không được chấp nhận cho release thường
- migration có thể chạy 3–8 phút trên production data
- deployment pipeline hiện chạy migration trước slot swap

## Bạn cần làm gì

1. Xác định failure modes của flow hiện tại.
2. Viết decision proposal trong `workspace/my-decision.md`.
3. Chọn một rollout strategy cho release có schema change.
4. Mô tả contract tương thích giữa old code, new code và database trong từng phase.
5. Định nghĩa rollback plan và trigger để abort release.
6. So sánh ít nhất hai alternative trước khi xem reference solution.

## Yêu cầu môi trường

Không cần Azure subscription hay local runtime. Đây là design-decision lab dựa trên production constraints.

## Chạy nhanh

1. Đọc constraints trong README.
2. Điền `workspace/my-decision.md`.
3. Chỉ sau khi chốt quyết định của bạn mới mở `solution/README.md`.

## Cách reproduce vấn đề

Không áp dụng. Lab không có deterministic runtime failure; mục tiêu là phân tích release contract và failure modes trước khi triển khai production.

## Những gì cần quan sát

- App slot có thể swap nhanh nhưng database state không swap theo application slot.
- Có khoảng thời gian old code và new code có thể cùng tồn tại quanh deployment/rollback.
- Migration duration dài hơn thời gian swap.
- Một rollback application không mặc định đồng nghĩa rollback database an toàn.

## Quy tắc làm lab

1. Ghi assumptions.
2. Liệt kê failure modes.
3. Đề xuất strategy.
4. Kiểm tra rollback compatibility.
5. Chỉ sau đó mới xem solution.

## Hints

Không có hint trực tiếp. Hãy kiểm tra compatibility theo từng phase của rollout.

## Reference Solution

> Spoiler: đây là một defensible solution, không phải kiến trúc duy nhất đúng.

- [Reference Solution](solution/README.md)

## Expected Results

Một decision tốt phải nêu rõ:

- migration sequencing
- backward/forward compatibility
- slot warm-up và health gate
- rollback boundary
- cách xử lý destructive schema change
- observability cần có trong rollout

## Estimated Time

45–75 phút.
