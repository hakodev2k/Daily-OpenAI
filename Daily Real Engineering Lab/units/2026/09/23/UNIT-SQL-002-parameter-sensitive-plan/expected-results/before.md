# Before
- Cả tenant nhỏ và tenant lớn trả kết quả đúng về mặt functional.
- Thứ tự call đầu tiên sau khi plan cache được làm sạch có thể ảnh hưởng plan được reuse.
- Với data skew lớn, một plan phù hợp cho cardinality nhỏ có thể không phù hợp cho cardinality lớn, hoặc ngược lại.
- Tập trung vào chênh lệch logical reads, operator choice và Estimated-vs-Actual rows; timing tuyệt đối có thể khác theo máy.