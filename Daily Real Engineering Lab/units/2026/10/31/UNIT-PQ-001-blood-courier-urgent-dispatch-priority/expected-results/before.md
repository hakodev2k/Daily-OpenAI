# Before — starter nguyên bản
- `urgent escalation changes dispatch order`: FAIL, yêu cầu BLD-B có thể được lấy trước BLD-A.
- `multiple escalations still dispatch exactly once`: FAIL ở yêu cầu khẩn nhất.
- Các case không liên quan có thể PASS.
- `reproduce.ps1` trả exit code 0 **chỉ khi** harness thực sự phát hiện `LAB_RESULT:FAIL`.
- Lỗi biên dịch không được coi là reproduce thành công.
