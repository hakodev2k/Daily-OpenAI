# Investigation guide

1. Chạy smoke và reproduce trên cùng máy, ghi exit code và counters.
2. Vẽ request chain: initial URI → response status/Location → next URI → final status.
3. Phân loại những header được phép/không được phép gửi tới origin mới. Không log bearer token.
4. Đề xuất thay đổi nhỏ nhất trong `PartnerGatewayClient` để đáp ứng functional contract **và** security contract.
5. Chạy verify. Nếu không thành công, kiểm tra: URL resolution, redirect status, header lifecycle, origin comparison, số hop và disposal.
6. Chỉ sau khi tự hoàn thành mới đọc `solution/README.md`.
