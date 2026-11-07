# Tình huống điều tra

Hệ thống ad-tech tính impressions theo các cửa sổ giờ không chồng lấn. Mỗi cửa sổ phải có một kết quả settlement, dù worker được đánh thức muộn. Một lần downstream bị chậm không được làm mất nghĩa vụ xử lý.

**SLA:** chốt đầy đủ mọi cửa sổ đến hạn theo thứ tự; một cửa sổ thất bại phải có thể được thử lại, không được tự coi là hoàn thành.

**Giới hạn lab:** thời gian nghiệp vụ được mô phỏng bằng `DateTimeOffset`; `PeriodicTimer` chỉ dùng để tạo bằng chứng trực quan. Không dùng dịch vụ cloud, không yêu cầu đồng hồ máy trùng timestamp trong fixture.

**Điều tra:** so sánh số cửa sổ đến hạn, số lần sink thực sự được gọi, và checkpoint trước/sau lỗi. Không kết luận từ một log thành công đơn lẻ.