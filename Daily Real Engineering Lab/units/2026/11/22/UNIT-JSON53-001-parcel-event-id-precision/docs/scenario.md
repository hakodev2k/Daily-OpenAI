# Incident brief
Đối tác phát ba scan event hợp lệ trong cùng một batch. Dashboard nội bộ chỉ hiển thị hai kiện. Nguồn phát không retry và không có event nào trùng theo payload gốc. SLA yêu cầu không mất scan hợp lệ. Điều tra tại ranh giới ingest trước khi đề xuất queue, retry hoặc database.
