# Senior extension — policy regression strategy

Thiết kế một CI contract test kiểm tra route inventory khi team thêm endpoint mới: mặc định deny anonymous, ngoại lệ public phải được review; đồng thời thử authenticated user không có quyền truy cập audit thuộc tenant khác. Phân biệt `401` và `403`; nêu giới hạn của fake handler trong lab và yêu cầu cho integration test với real authentication scheme. Không cần triển khai Azure.
