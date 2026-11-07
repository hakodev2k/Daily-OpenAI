# Senior challenge — thay đổi yêu cầu

Một đối tác cũ không gửi `UtcOffsetMinutes`, nhưng SLA yêu cầu không âm thầm đặt sai lịch. Đề xuất một rollout gồm: contract version, trạng thái từ chối cho ambiguous input, hướng dẫn đối tác sửa payload, telemetry, backward compatibility và cách replay an toàn khi họ gửi lại. Không tự chọn giờ thay cho đối tác. Nêu rõ phương án khi timezone database thay đổi sau khi lịch được tạo.
