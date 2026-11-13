Gắn quyền release với một biến ownership chỉ được set sau khi await acquire thành công. Giữ cleanup `active` riêng cho code đã vào critical section.
