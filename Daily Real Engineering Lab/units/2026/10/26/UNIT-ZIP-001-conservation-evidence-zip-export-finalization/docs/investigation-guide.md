# Investigation guide (không chứa lời giải)

1. So sánh thời điểm producer cho rằng export thành công với thời điểm consumer mở ZIP.
2. Kiểm tra reader dừng trước hay sau khi truy cập từng entry; một file có đủ payload chưa chắc đã là một container hợp lệ.
3. Tìm hiểu cách thư viện quản lý vòng đời của đối tượng đang ghi; phân biệt “ghi dữ liệu entry” và “hoàn tất container”.
4. Kiểm tra cả input rỗng để loại bỏ giả thuyết liên quan riêng đến nội dung tài liệu.
5. Thử một thay đổi tối thiểu rồi chạy `verify.ps1`; không sửa `Probe` hay giảm mức kiểm tra để làm test xanh.

Ghi hypothesis, evidence, điều kiện loại trừ và kết quả thử nghiệm trong `workspace/my-investigation.md`.
