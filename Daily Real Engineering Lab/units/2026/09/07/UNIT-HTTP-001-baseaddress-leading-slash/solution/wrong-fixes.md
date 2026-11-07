# Wrong Fixes

## Thêm retry cho 404

Retry không sửa URI sai. Bạn chỉ gửi lại cùng request sai nhiều lần hơn.

## Đổi DNS hoặc reverse proxy route ngay lập tức

Evidence ở handler đã cho thấy client tạo sai URI trước khi network transport xảy ra, nên thay đổi infrastructure sẽ che giấu bug thay vì sửa contract.

## Nối URL bằng string

Có thể làm case này chạy nhưng tạo thêm risk về slash, escaping, query string và absolute/relative semantics.

## Hard-code absolute URL ở từng call site

Có thể bypass lỗi hiện tại nhưng làm mất giá trị của centralized `HttpClient` configuration và tăng configuration drift giữa environment.
