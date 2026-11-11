# Investigation guide
- Chạy baseline hai lần để xác định tính ổn định.
- Vẽ sơ đồ đối tượng `PrinterRegistry`, `LabelService` và vòng đời của chúng.
- So sánh đường `Update` với đường `Render` tại hai thời điểm.
- Kiểm tra xem dữ liệu quan sát là giá trị hiện hành hay bản sao tại một thời điểm.
- Viết một test contract cho nhãn trước/sau refresh; xác định yêu cầu atomic snapshot khi cập nhật nhiều field.

