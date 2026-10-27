# Senior Challenge — thay đổi requirement

Business cập nhật:

1. Ca đêm cần tối thiểu **2 kiểm lâm viên**; ca ngày cần tối thiểu 1.
2. Ứng dụng chạy 3 replicas; hai lệnh nghỉ có thể vào hai replicas khác nhau.
3. Có lệnh điều chuyển người giữa hai ca; hai ca phải giữ đúng minimum coverage.
4. Retry từ mobile có thể gửi lại cùng `requestId` sau timeout.

Hãy thiết kế một transaction boundary tại database, idempotency contract, conflict response, và integration test chạy bằng hai DB connections thật. So sánh `SERIALIZABLE`, CAS trên aggregate và command serialization. Nêu rõ tình huống nào khiến phương án bạn chọn không còn phù hợp.
