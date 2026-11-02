# Before

Chạy reproduce.ps1 trả 0 sau khi xác nhận chương trình xuất LAB_RESULT=EXPECTED_FAILURE và exit code 3.

- Initial: A valid:v1, B valid:v1, C review:v1.
- Phase 1: A còn valid:v1, B valid:v2, C còn review:v1; D vắng mặt.
- Phase 2: B được xóa khỏi index nhờ notification, nhưng A/C vẫn cũ và D/E chưa có.
- notificationsReceived=5, maintenanceTicks=3, không có exception.

Nếu không tái hiện, kiểm tra SDK 8+ và bảo đảm starter chưa được sửa.
