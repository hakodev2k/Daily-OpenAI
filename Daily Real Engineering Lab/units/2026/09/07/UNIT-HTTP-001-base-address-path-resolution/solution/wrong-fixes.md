# Wrong Fixes

## Retry request nhiều lần
Retry không sửa URI sai. Bạn chỉ gửi cùng request tới cùng endpoint sai nhiều lần hơn.

## Tăng timeout
Không có timeout symptom trong lab này. Response `404` đã quay về thành công ở transport layer.

## Dùng absolute URI ở mọi call site
Có thể làm một request chạy đúng nhưng tạo duplication cấu hình endpoint và làm giảm khả năng quản lý client tập trung.

## Thêm `/` vào đầu relative path
Relative path kiểu `/orders/42` bắt đầu từ root của authority và vẫn loại bỏ `/api/` khỏi base path.
