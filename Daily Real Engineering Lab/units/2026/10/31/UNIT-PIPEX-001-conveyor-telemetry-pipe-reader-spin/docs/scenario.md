# Scenario
Một controller gửi bản tin `CONVEYOR|42` kết thúc newline. Fixture chia thành hai fragment `CONVEYOR|4` và `2\\n`. Chỉ fragment đầu có mặt khi bắt đầu đọc.

SLO: không spin trên dữ liệu không đổi, không mất prefix, trả đúng một frame khi đủ delimiter. `FrameProbe` mô phỏng instrumentation, không phải implementation fix. Các giả thuyết cạnh tranh gồm network latency, UTF-8 decoding, queue congestion và parser progress.
