# UNIT-ARCH-001 — Chọn Runtime Cho Background Processing

## Mục tiêu

Ra quyết định kỹ thuật giữa `BackgroundService` trong web app, dedicated .NET Worker và Azure Functions cho một workload nền có yêu cầu thực tế nhưng chưa cần kiến trúc phức tạp.

## Bối cảnh thực tế

Một CMS cần xử lý metadata cho media vừa upload: đọc file, trích metadata, tạo thumbnail request và cập nhật trạng thái. Hiện tại team đang cân nhắc ba hướng triển khai nhưng chưa có tiêu chí rõ ràng.

## Bạn cần làm gì

1. Đọc constraints trong `docs/scenario.md`.
2. Ghi assumptions và decision criteria vào `workspace/my-decision.md`.
3. Đánh giá cả ba phương án về reliability, deployment coupling, scale, operational complexity và cost.
4. Chọn một phương án cho Release 1 và nêu điều kiện khiến bạn đổi quyết định sau này.
5. Chỉ sau đó mới xem reference solution.

## Những gì cần quan sát

Lab này không có một đáp án duy nhất. Một quyết định tốt phải bám constraints, nêu failure modes và tránh chọn công nghệ chỉ vì nó "senior" hơn.

## Quy tắc làm lab

1. Không mở solution trước.
2. Không giả định scale lớn hơn dữ liệu đề bài.
3. Phải nêu ít nhất một nhược điểm của phương án bạn chọn.
4. Phải định nghĩa trigger để re-evaluate kiến trúc.

## Reference Solution

> **Spoiler:** xem sau khi hoàn thành decision record.

- [Reference Solution](solution/README.md)

## Estimated Time

35–50 phút.
