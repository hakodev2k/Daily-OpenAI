# Hint 3 — Gần lời giải

Kiểm tra \`BoundedChannelOptions.FullMode\`: chính sách nào làm writer chờ khi không còn chỗ, thay vì loại bỏ event? Sau đó xác nhận \`WriteAsync\` vẫn quan sát \`CancellationToken\` khi đang chờ.
