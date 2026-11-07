# Requirement change — đối tác cập nhật feed SLA

Sau khi bạn hoàn thành ADR, Product đưa thêm thông tin:

> Đối tác vẫn giữ 48h retention nhưng xác nhận feed có thể **chậm đến 26 giờ** trong giai đoạn cuối tháng; webhook retry vẫn giữ nguyên chữ ký cũ. Product không chấp nhận tăng 24h recovery bound và Security không nới cutoff 09:30.

Yêu cầu: cập nhật ADR. Phương án bạn chọn còn chứng minh được 24h recovery không? Có thể bổ sung cơ chế nào được partner hỗ trợ để khôi phục bound (ví dụ re-sign retry, dedicated authenticated backfill endpoint với SLA mới)? Nêu rõ điểm cần renegotiate, interim incident policy, NO-GO criteria và rủi ro đối với parcel release.
