# Sau lab: identity, equality và mutability

Một hash collection yêu cầu hash của key không thay đổi trong suốt thời gian key nằm trong collection. `record` tạo value equality dựa trên thành phần dữ liệu; thay đổi dữ liệu tham gia equality/hash sau insert phá vỡ hợp đồng tra cứu. Cách thiết kế thường dùng là ID ổn định làm khóa, thuộc tính vận hành thay đổi trong value. Hãy tự bổ sung test cho hai kiện có cùng route và hai lần đổi route liên tiếp.
