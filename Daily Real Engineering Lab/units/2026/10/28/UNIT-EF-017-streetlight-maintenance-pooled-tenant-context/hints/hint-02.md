# Hint 02 — Vòng đời
Theo dõi vòng đời của WorkOrdersContext khi factory tạo và trả object về pool. Đối chiếu vòng đời đó với vòng đời của tenant identity mà request hiện tại cung cấp. Kiểm tra liệu EF Core có tự reset mọi property do ứng dụng tự định nghĩa hay không.
