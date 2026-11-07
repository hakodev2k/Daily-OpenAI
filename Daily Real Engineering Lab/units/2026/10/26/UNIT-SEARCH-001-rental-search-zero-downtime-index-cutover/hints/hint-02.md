# Hint 02 — Ranh giới dữ liệu

Tại thời điểm bắt đầu copy, PostgreSQL vẫn thay đổi. Hãy chỉ rõ một mốc có thể replay, số event tích lũy trong backfill và tốc độ **ròng** để thu hẹp backlog. Nếu replay <= tốc độ phát sinh thì liệu có thời điểm tự bắt kịp hay không?
