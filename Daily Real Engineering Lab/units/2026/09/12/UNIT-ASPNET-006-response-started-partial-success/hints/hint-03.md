# Hint 3

Nếu business contract yêu cầu “hoặc file hoàn chỉnh, hoặc error response”, hãy trì hoãn việc commit success response cho tới khi phần công việc có thể fail đã hoàn tất, hoặc buffer kết quả trong một boundary phù hợp trước khi copy ra response.