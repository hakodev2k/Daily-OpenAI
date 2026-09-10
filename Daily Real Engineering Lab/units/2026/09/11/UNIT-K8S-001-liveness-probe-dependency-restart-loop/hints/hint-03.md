# Hint 03

Dependency health thường phù hợp với readiness hơn liveness. Với transient database outage, process vẫn có thể sống và chờ dependency phục hồi thay vì tự làm tình hình tệ hơn bằng restart loop.
