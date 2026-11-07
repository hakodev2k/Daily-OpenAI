# Senior challenge — live pagination

Thay `RotatingReceiptFeed` bằng một API trả 50 trang, mỗi trang 1.000 phiếu. Producer có thể chèn phiếu giữa các page requests. Hãy đề xuất và so sánh: snapshot token, watermark + immutable event log, cursor ổn định, và reconciliation cuối kỳ.

Acceptance: mô tả được consistency guarantee, memory budget 32 MB, timeout 5 giây, retry khi một trang 503, idempotency theo receipt ID, và cách audit đối soát khi phiếu đến muộn. Không được mặc định một lượt `foreach` giải quyết được consistency giữa các page.
