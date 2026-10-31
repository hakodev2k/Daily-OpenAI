# Scenario — Monorepo change detection

CI nhận `BaseRef`, `HeadRef` và trả danh sách path để ánh xạ sang project cần build/test.

- **case-a:** feature thêm Enrollment, main chỉnh Clock.
- **case-b:** feature sửa Approval, main thêm ReleaseNote.
- **case-c:** feature không có commit mới, main sửa Formatter.

Acceptance: chỉ trả file thay đổi thuộc feature so với lịch sử chung; không bỏ sót file feature sửa, không báo file chỉ thuộc main. `setup.ps1` tái tạo fixture độc lập với lịch sử repository thật.
