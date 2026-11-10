# Hợp đồng tính tiền

- Mỗi dòng tín chỉ được ghi sổ độc lập bằng VND với 2 chữ số thập phân trong bài mô phỏng.
- Giá trị dòng = `quantity * unitPrice` dạng `decimal`.
- Mỗi dòng làm tròn **2 chữ số, midpoint away from zero** theo hợp đồng đối soát.
- Tổng hóa đơn bằng **tổng các dòng đã làm tròn**, không làm tròn lại tổng thô.
- Dòng âm là credit, xử lý đối xứng với dòng dương.
- Input cố định (quantity=1):
  - A: +0.005 VND; B: +0.005 VND; C: -0.005 VND => tổng kỳ vọng +0.01 VND.
  - D: +0.015 VND; E: +0.015 VND => +0.04 VND.
  - F: -0.015 VND; G: -0.015 VND => -0.04 VND.
- Không dùng `double` để tính tiền. Không làm mất tính độc lập theo dòng khi tái cấu trúc.
