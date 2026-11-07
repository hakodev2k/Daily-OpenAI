# Hint 2

Peek-lock style messaging tách việc nhận message khỏi việc hoàn tất message. Nếu handler còn làm việc sau khi lock window kết thúc, broker có thể giao message lại. Kiểm tra contract nào cho phép duy trì ownership khi công việc vẫn đang chạy.
