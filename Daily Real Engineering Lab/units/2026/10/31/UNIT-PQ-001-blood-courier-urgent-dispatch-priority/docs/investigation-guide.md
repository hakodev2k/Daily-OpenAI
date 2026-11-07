# Investigation guide (không spoiler)

1. Chạy `./scripts/reproduce.ps1`; chép chính xác các case PASS/FAIL.
2. Lập bảng thời điểm tiếp nhận, thay đổi mức khẩn và thứ tự thực tế.
3. Ghi hai giả thuyết độc lập: vấn đề ở việc cập nhật dữ liệu, ở cấu trúc lập lịch, hay ở bước lấy công việc?
4. Dùng breakpoint tại `Register`, `Escalate`, `DispatchNext`; kiểm tra dữ liệu được giữ ở mỗi bước.
5. Đề xuất invariant: tại thời điểm dispatch, chỉ chọn yêu cầu chưa xử lý có mức khẩn mới nhất, không phát trùng.
6. Sửa `starter/DispatchQueue.cs`, chạy `./scripts/verify.ps1`. Thử nâng mức nhiều lần và yêu cầu đồng mức.
7. Sau khi hoàn tất, mở reference và đánh giá chi phí bộ nhớ, thao tác và chiến lược compact.
