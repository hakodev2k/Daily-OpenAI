# Acceptance matrix — đánh giá trước khi xem lời giải

| Case | Input / timing | Điều kiện pass |
|---|---|---|
| A1 | 09:10 chưa nhận R3 | Không phát CLEAR không thể thu hồi như trạng thái cuối cùng |
| A2 | 09:13 nhận R3=510, measuredAt 09:08:30 | Cảnh báo nguy hiểm được phát chậm nhất 09:15; trace đến readingId |
| A3 | 09:14 nhận duplicate r3-001 revision 1 | Không nhân đôi phép đo hoặc gửi hai thông báo cùng revision |
| A4 | 09:17 revision 2 = 515 | Tính lại cửa sổ và phát bản revision mới nếu payload thay đổi |
| A5 | 09:20 R4 tới muộn > 8 phút | Có late-data metric và đường xử lý/reconciliation; không drop im lặng |
| A6 | Trạm offline 25 phút | Không coi thiếu dữ liệu là bằng chứng an toàn; degraded state hiển thị |
| A7 | Crash trước publish và replay | Cùng dữ liệu cho cùng windowId và revision logic, không mất dấu audit |
| A8 | Requirement 45 phút | Có policy theo station và tác động SLO/cost được phê duyệt rõ |

Đầu ra cần nộp: invariant, sơ đồ luồng, event-time vs processing-time timeline, watermark strategy, revision semantics, bảng trade-off A/B/C, failure-path, telemetry, rollback, và 3 test replay với expected outcome.