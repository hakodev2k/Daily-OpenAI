# Before — trạng thái starter

- \`./run.ps1\` in hai scenario và bảng \`actual\`/\`expected\` theo cửa sổ UTC.
- Ca cao điểm: 10:00=+1, 10:05=+1, 10:10=0; expected lần lượt +1, 0, +1.
- Tổng ca cao điểm vẫn bằng +2; do đó kiểm tra tổng đơn thuần không phát hiện lỗi.
- Scenario có offset và ranh giới thời gian cũng có ít nhất hai cửa sổ sai.
- \`./reproduce.ps1\` exit 0 khi nhận diện được symptom; \`./verify.ps1\` exit 1.
