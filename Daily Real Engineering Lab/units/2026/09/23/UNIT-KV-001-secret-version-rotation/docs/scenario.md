# Scenario

Payment API dùng credential lấy từ Azure Key Vault để gọi một payment provider. Quy trình rotation tạo một secret version mới và provider đồng thời ngừng chấp nhận credential cũ. Không có application deployment trong cửa sổ incident.

Triệu chứng: sau rotation, instance `api-a` liên tục nhận 401; instance `api-b` được restart lúc 22:07 bắt đầu thành công ngay sau khi khởi động lại. Cả hai vẫn có network connectivity tới provider.

Nhiệm vụ của bạn là xây dựng hypothesis, dùng evidence để loại trừ nguyên nhân, xác định lifecycle boundary bị sai và đề xuất verification cho fix.