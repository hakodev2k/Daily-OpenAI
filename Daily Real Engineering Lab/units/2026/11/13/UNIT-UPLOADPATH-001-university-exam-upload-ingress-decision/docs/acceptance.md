# Quality gate — design replay

- [ ] Có tính đúng 30 attempts/s và ~3.02 Gbit/s peak mean payload, nêu giả định.
- [ ] Có phân biệt 51 buffers là upper bound lý thuyết, không phải concurrency an toàn.
- [ ] Ít nhất ba alternatives, có trade-off và trigger đảo quyết định.
- [ ] Có 4 hypotheses và evidence để falsify.
- [ ] Có sequence authorize/upload/finalize/receipt, server ownership checks.
- [ ] Có chính sách deadline, idempotency, scan, token scope, retention.
- [ ] Có failure matrix ít nhất 7 case.
- [ ] Có rollout/canary, load test và rollback.
- [ ] Không tự nhận Blob throughput chưa đo là chắc chắn đủ.
- [ ] Không biến lab generation thành learner mastery.
