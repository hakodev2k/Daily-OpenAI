# Hint 3

Một hướng sửa là biểu diễn periodic work như một vòng lặp async tuần tự: chờ tới tick, `await` toàn bộ work, rồi mới chờ tick tiếp theo. Hãy nghĩ thêm về shutdown/cancellation nếu áp dụng trong `BackgroundService` thực tế.
