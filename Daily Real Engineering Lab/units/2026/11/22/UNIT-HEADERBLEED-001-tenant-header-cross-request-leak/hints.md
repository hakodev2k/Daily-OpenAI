# Gợi ý tăng dần

1. Dữ liệu nào thuộc vòng đời của một HTTP request, dữ liệu nào thuộc vòng đời của client?
2. Quan sát giá trị header ngay tại `RecordingHandler.SendAsync` và so với thời điểm `Prepare`.
3. Thử tạo `HttpRequestMessage` riêng cho mỗi lần gửi và đặt header trên chính request; giữ lại HttpClient dùng chung.
