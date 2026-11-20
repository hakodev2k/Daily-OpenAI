# Acceptance criteria

- [ ] Ba giả thuyết, evidence ủng hộ/phản bác và phép thử phân biệt.
- [ ] Phân biệt tag, digest, label, imageID; xác nhận full digest trước khi deploy.
- [ ] Containment trong RTO 15 phút, canary, rollback, bảo vệ dữ liệu.
- [ ] Build-once/promote-many, provenance, signature, registry retention, quyền ghi tag.
- [ ] Verify runtime digest, golden payload, error/latency và đối soát dữ liệu.
- [ ] So sánh ít nhất hai lựa chọn và failure path nếu digest cũ bị GC.
- [ ] Không dùng digest rút gọn minh họa để triển khai thật.

Rubric: evidence 25%, containment 25%, prevention 25%, verification/trade-offs 25%. Không đánh dấu learner completed.
