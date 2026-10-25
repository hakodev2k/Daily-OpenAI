# Before (starter gốc)
- smoke: 202, exit 0.
- reproduce: exception phía client sau response 503; fake partner ghi nhận đúng 1 call; exit 0 vì symptom được tái hiện.
- verify: FAIL và exit 1; case 503→202 không đạt.
- Không yêu cầu khớp thời gian thực thi; các assertions hoàn toàn deterministic.
- Nếu reproduce không xuất hiện, kiểm tra rằng bạn chưa sửa starter/TransferDispatcher.cs.
