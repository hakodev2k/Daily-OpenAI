# Incident evidence (synthetic)

- Batch A: dữ liệu khảo sát âm thanh có nhiều dòng; downstream đôi lúc giải nén đọc được payload nhưng reject artifact.
- Batch B: file rỗng cũng bị reject; đây không phải lỗi một dòng CSV cụ thể.
- Nhận định ban đầu của nhóm vận hành: "network upload bị cắt" hoặc "nội dung Unicode".
- Harness cố ý báo ba thuộc tính riêng: `header`, `footer`, `roundtrip`.
- Không có timing benchmark giả lập. Mọi số byte và kết quả thực tế phải được thu thập bằng script trên máy người học.
