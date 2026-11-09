# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Dashboard báo 240/240 active dù relay G-19 lỗi; G-07 phát còi muộn sau ALL_CLEAR.

## 2. Evidence
Timeline: BROKER DELIVERED 240 queues không chứng minh gateway đã nhận; ACCEPTED G-07 trước relay controller online; G-19 sensor fault; ALERT-41 replay sau ALLCLEAR-42.

## 3. Root cause
Một trạng thái `delivered/accepted` được diễn giải là `actuated`, trong khi lệnh cũ không được chặn tại gateway bằng version fence và validity check. Đây là lỗi hợp đồng distributed control, không đơn thuần broker reliability.

## 4. One defensible design
- Trung tâm phát `{commandId,region,epoch,sequence,action,issuedAt,expiresAt,signature}` với sequence tăng đơn điệu trong epoch; gateway lưu durable `lastAppliedEpoch/Sequence` và `lastObservedEpoch/Sequence` trước khi cho phép side effect. Chính sách reset epoch phải được ký/ủy quyền, không tự reset sau reboot.
- ACK có 2 loại: `Accepted{commandId,sequence,deviceId,receivedAt}` và `ActuationObserved{commandId,sequence,deviceId,relayState,observedAt,sensorHealth}`. Chỉ xác nhận confirmed khi sensor phản hồi tương ứng; relay ON chưa chứng minh âm thanh, vì vậy vùng rủi ro cao vẫn cần kiểm tra ngoài băng.
- Dashboard dùng `DISPATCHED → ACCEPTED → ACTUATION_CONFIRMED`, cộng `UNKNOWN`, `FAILED`, `EXPIRED`, `SUPERSEDED`. Mất heartbeat làm trạng thái quan sát trở thành UNKNOWN, không giữ green vĩnh viễn.
- ALL_CLEAR sequence 42 làm mọi ALERT sequence ≤41 bị superseded. Reject lệnh cũ ngay tại gateway dù broker redeliver. Side effect idempotent theo commandId+deviceId+epoch; audit append-only.
- Clock skew ±90s: không dùng riêng gateway wall-clock để quyết định an toàn. Gateway dùng thời gian đồng bộ có confidence/uncertainty; khi không thể chứng minh lệnh còn hiệu lực, không thực thi lệnh từ xa đã lưu quá lâu, chuyển UNKNOWN và báo operator. Chính sách cảnh báo local độc lập được thiết kế riêng.
- Local sensor trigger: signed policy cài trước, thresholds/hysteresis, local priority và physical override; event log durable, upload khi reconnect. Trung tâm hiển thị trạng thái `LOCAL_ACTIVE` và reconcile, không giả định ALL_CLEAR remote đã được áp dụng.

## 5. How to verify
Chạy `pwsh -File reproduce.ps1` để đọc evidence. Điền `workspace/decision.md`, chạy `pwsh -File verify.ps1`, sau đó đánh giá A1–A9 thủ công bằng mô hình state machine, message traces hoặc simulator do bạn bổ sung. Script chỉ kiểm tra độ đầy đủ tài liệu, không kiểm chứng safety của thiết kế.

## 6. Alternatives
A. Chỉ broker ACK: rẻ, latency thấp, nhưng không đáp ứng safety contract.
B. Gateway ACK + relay feedback: phù hợp ngân sách, vẫn có blind spot âm thanh.
C. Thêm cảm biến âm thanh + đội kiểm tra hiện trường: quan sát mạnh hơn, tốn thiết bị và vận hành.

## 7. Wrong / tempting fixes
- Tăng broker retries: làm lệnh trễ xuất hiện nhiều hơn nếu thiếu fence.
- Dashboard đổi nhãn accepted thành active: che lỗi bằng UI.
- Tin vào timestamp gateway: sai khi clock skew hoặc mất NTP.
- Chặn tất cả offline behavior: tránh stale remote command nhưng có thể mất cảnh báo cục bộ khi thiên tai.

## 8. Production implications
Dùng signed command, key rotation, per-region sequence authority, durable gateway journal, heartbeat age SLO, alarms khi UNKNOWN, runbook manual override, diễn tập định kỳ và kiểm toán 90 ngày. Thiết bị điều khiển an toàn phải được thử nghiệm thực địa và đánh giá theo quy định địa phương; lab này chỉ là mô phỏng.

## 9. Trade-offs
Độ trễ confirmed phụ thuộc sensor; bảo đảm an toàn cần trạng thái UNKNOWN thay vì green giả. Local autonomy tăng availability nhưng đòi policy quản trị, audit và giải quyết conflict.

## 10. Senior engineer should notice
ACK của transport không phải bằng chứng effect; exactly-once delivery không đồng nghĩa exactly-once physical actuation; thứ tự phải được cưỡng chế tại ranh giới side effect; missing evidence là một trạng thái sản phẩm cần hiển thị.

## Tài liệu đọc thêm
- [Microsoft — Idempotency patterns in distributed applications](https://learn.microsoft.com/en-us/azure/architecture/patterns/idempotent-consumer)
- [Microsoft — Retry pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/retry)
