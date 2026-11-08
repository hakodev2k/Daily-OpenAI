# Acceptance checklist (tự đánh giá)

- [ ] Chỉ rõ ít nhất 3 failure hypotheses và bằng chứng phân biệt.
- [ ] Với timeline hiện tại, giải thích tại sao K2 token 09:01 có thể thất bại ở cả A/B/C.
- [ ] Đề xuất thời điểm prepublish K2 sớm hơn activation ít nhất một chu kỳ refresh lớn nhất cộng margin; không coi cache TTL là SLA refresh tuyệt đối.
- [ ] Đề xuất retire K1 sau khi token K1 cuối cùng hết hạn cộng clock skew và propagation margin.
- [ ] Chỉ rõ điều kiện không thể đảm bảo zero-rejection khi partner mất kết nối hoặc không tuân thủ refresh contract.
- [ ] Xác minh `kid`, signature, algorithm allowlist, issuer, audience, exp, nbf và key provenance.
- [ ] Có metric `unknown_kid` theo partner và tỷ lệ verification failure; có go/no-go threshold và rollback.
- [ ] Với compromised K1, nêu lựa chọn revoke ngay, chặn lạm dụng, truyền thông và tác động token hợp lệ.
- [ ] Phân biệt rollback issuer signing key với việc tiếp tục tin khóa bị lộ.
- [ ] Trình bày một thay đổi requirement và cách thiết kế thích ứng.
