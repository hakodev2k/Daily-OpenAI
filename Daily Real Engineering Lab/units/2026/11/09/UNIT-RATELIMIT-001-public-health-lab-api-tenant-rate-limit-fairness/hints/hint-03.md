# Hint 03
Per-instance counters không đồng nghĩa với quota global. Cân nhắc một admission coordinator atomic, token bucket có giới hạn burst, và policy riêng khi coordinator bị lỗi.