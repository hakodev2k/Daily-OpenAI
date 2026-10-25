# Senior Challenge — đồng thời và độ bền

Sau khi `verify` xanh, thiết kế phương án production cho hai cán bộ cùng approve `P-204` từ `Version=7`.

Yêu cầu: chỉ một transition thành công, người còn lại nhận conflict rõ ràng; ghi audit đúng một lần; không dùng lock trong process nếu có nhiều pod. Đề xuất optimistic concurrency token hoặc conditional update, transaction boundary, và integration tests với database local. Nêu trade-offs so với distributed lock và cách xử lý retry khi client timeout sau commit.

Không thay đổi bài core hoặc claim bài core đã chứng minh tính đúng đắn dưới concurrency; harness hiện tại chỉ chứng minh persistence contract tuần tự.
