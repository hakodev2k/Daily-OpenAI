# Hint 2 — subsystem

Quan sát cách `Dictionary<string, ...>` đánh giá key equality và hash. Hãy thử so sánh `"INSP-204"` với `"insp-204"` dưới các locale được mô phỏng. Vì sao cả hai worker đều xử lý cùng feed nhưng số key khác nhau?
