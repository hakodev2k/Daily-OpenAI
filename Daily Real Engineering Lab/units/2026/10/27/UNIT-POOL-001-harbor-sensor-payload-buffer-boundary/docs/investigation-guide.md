# Investigation guide

1. Chạy reproduce, lưu declared length và decoded length.
2. Đặt các giả thuyết: sai nguồn dữ liệu, Base64 decoder, cấp phát buffer, HTTP framing.
3. Theo dõi byte count trước read, sau read và sau encode/decode.
4. Chứng minh case 32 bytes không đại diện cho các gói ngắn.
5. Chỉnh starter tối thiểu; chạy verify với 0, 4, 5, 31, 32 bytes.
6. Đánh giá hệ quả dữ liệu dư và regression tests.

**Done:** 5/5 case pass, pool được trả đúng một lần/case, không thay đổi harness.
