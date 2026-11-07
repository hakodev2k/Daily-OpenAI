# After — hợp đồng nghiệm thu

- \`./verify.ps1\` exit 0, in \`VERIFIED\`.
- Ca cao điểm: 10:00=+1, 10:05=0, 10:10=+1; tổng +2.
- Scenario offset: 10:00=+1, 10:05=0; tổng +1, không sinh thêm khung 10:10.
- Timestamp biểu diễn cùng instant nhưng offset khác nhau phải vào cùng cửa sổ UTC.
- Event ở đúng 10:05:00 UTC thuộc cửa sổ bắt đầu 10:05, không thuộc cửa sổ trước.
- Không đổi \`starter/Program.cs\` hoặc expected fixtures để làm xanh test; verification kiểm tra code learner đã sửa.
