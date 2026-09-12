# Hint 3

Elasticsearch search là near-real-time. Với workflow thật sự cần search thấy document trước khi tiếp tục, xem xét refresh policy ở đúng write boundary; phân biệt `wait_for`, explicit refresh và việc chỉ sleep/poll mù.
