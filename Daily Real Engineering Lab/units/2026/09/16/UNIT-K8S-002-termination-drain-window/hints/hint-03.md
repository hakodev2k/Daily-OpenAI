# Hint 3

Tách `ready for new traffic` khỏi `process still running`. Khi termination bắt đầu, admission signal nên thay đổi trước shutdown, sau đó mới có drain window cho in-flight work.