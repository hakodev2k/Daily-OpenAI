# Reference Solution — xem sau khi tự làm

Đây là **một phương án có thể bảo vệ bằng evidence**, không phải kiến trúc duy nhất.

## 1. Symptoms và evidence

Snapshot: North 74/2/3/1, Central 75/3/2/0, South 75/3/2/0 (succeeded/failed/offline/in-progress). Tổng 224 succeeded, 8 failed, 7 offline, 1 in progress; terminal = 232. Pilot failure rate = **8/232 = 3,45%**, vượt 2% sau ≥100 terminal. Quyết định 09:05: **NO-GO**, pause mở wave và triage. Không đưa offline vào mẫu số lỗi firmware. Timeline chỉ là sample.

c-014 attempt=1 SUCCEEDED đến sau attempt=2 FAILED; n-044 attempt=2 SUCCEEDED lặp lại. Event count không bằng unique device outcome.

## 2. Root cause / architecture gap

Dùng command dispatch hoặc event count làm source of truth mà không có durable campaign gate, per-device attempt fencing và state reconciliation sẽ khiến stale ACK/duplicate làm sai trạng thái, hoặc campaign tiếp tục sau khi gate đã fail. Offline command và rollback không thể được coi là hoàn tất chỉ vì control plane gửi yêu cầu.

## 3. One defensible architecture

**Central campaign intent + regional pull delivery**:

1. Central service giữ immutable signed firmware manifest, cohorts, policy, campaign epoch và ACTIVE/PAUSED/CLOSED state.
2. Regional controller giữ per-device state, command leases và per-site admission. Device chỉ outbound poll để lấy offer, phù hợp NAT/offline.
3. Scheduler cấp INSTALLING bằng atomic conditional transition bảo đảm 1/site và 240 global; khi lease hết hạn vẫn phải reconcile observed state trước khi cấp slot cho thiết bị khác.
4. Device agent kiểm tra signature, digest, hardware compatibility, minimum firmware version, anti-rollback floor; ACK gồm eventId, campaignId, deviceId, attemptId, sequence, digest.
5. Durable inbox deduplicate eventId, reject stale attempt/sequence khỏi current state nhưng giữ audit; chỉ terminal state của attempt hiện tại được aggregate.
6. Regional raw logs ở nguyên vùng; global metrics nhận aggregates. Có outbox cho event delivery và replay.

Data flow: operator → central campaign intent → regional scheduler → outbound device poll → signed manifest → install/health check → durable ACK → regional state → aggregated metrics.

## 4. State machine / pause / rollback

ELIGIBLE → OFFERED → DOWNLOADING → INSTALLING → VERIFYING → SUCCEEDED. Branches: OFFLINE_PENDING, FAILED, ROLLBACK_PENDING, ROLLED_BACK, MANUAL_INTERVENTION, HALTED. Chỉ một active attempt/device. Terminal outcome không được ghi đè bởi event attempt cũ.

Emergency pause tăng campaign epoch, scheduler không phát command mới, device poll không nhận offer cũ. Lệnh đã INSTALLING có thể hoàn thành hoặc fail; không hứa hủy tức thì. Offline giữ pending intent. Dual-bank rollback cần health-check ACK mới coi là ROLLED_BACK; single-bank chuyển MANUAL_INTERVENTION và field runbook.

## 5. Capacity và rollout gates

Waves: pilot 240, 1.200, 4.800, phần còn lại, cân bằng region/hardware/site. Gate theo terminal failure rate, site availability, power-output health, timeout confidence và manual approval.

Full/delta mix: 24.000 × (0,8 × 12 MiB + 0,2 × 45 MiB) = **446.400 MiB ≈ 435,94 GiB**. 320 GiB/day ⇒ lower bound 1,36 ngày, chưa tính retransmits, overhead, offline. Full-only: 24.000 × 45 MiB ≈ 1.054,69 GiB, lower bound 3,30 ngày. 240 concurrent, giả sử 8 phút/install ⇒ lý thuyết 1.800 installs/giờ; không phải SLO.

Với 6.000 priority devices và 30% offline trong một giờ, 4.200 online tại snapshot; 95% online eligible là 3.990. Không cam kết 6.000/6.000 trong 48 giờ nếu không có connectivity guarantee. Kiểm chứng phân bố site và delta eligibility trước khi chốt.

## 6. Security / observability / operations

Signed manifest, pinned trust roots, hash check, anti-rollback, per-device authorization, operator audit, rate-limited polling. Metrics: terminal failure rate per cohort/region/hardware, offline backlog age, site availability, rollback pending age, stale-ACK rejects, egress/day. Alert có nguồn độc lập, audit go/no-go, runbook pause/region isolate/manual recovery.

## 7. Alternatives / trade-offs

**A — centralized push scheduler:** đơn giản về control plane nhưng device không hỗ trợ inbound, network flaps và blast radius lớn; chỉ phù hợp nếu có gateway push đáng tin.

**B — central intent + regional pull (đề xuất):** hợp offline/NAT và residency, giới hạn blast radius; đổi lại cần durable leases, state reconciliation, eventual consistency và vận hành regional controllers.

**C — managed OTA platform:** có thể giảm vận hành nhưng phải kiểm tra single-bank recovery, residency, signing, attempt fencing, cost ≤4.000 USD/month và vendor lock-in.

## 8. Wrong / tempting fixes

- Đếm offline như FAILED hoặc SUCCEEDED: đánh tráo chất lượng firmware với connectivity.
- Retry mọi command khi mất ACK: có thể cài lặp hoặc ghi đè attempt.
- Nút rollback toàn fleet: không thể đảo ngược thiết bị offline hay single-bank ngay.
- Chỉ dùng distributed lock TTL: expiry không chứng minh device đã dừng install.
- Mở wave vì 224/240 đã thành công: bỏ qua gate 2% và terminal denominator.
- Tăng worker để đạt 48h: không giải quyết egress cap, site invariant hoặc offline.

## 9. Verification

Walkthrough A1–A6 trong acceptance matrix. Production cần fault injection: reordered/duplicate ACK, network partition, scheduler failover, paused campaign reconnect, single-bank recovery, tampered signed manifest. Test shadow-mode trên synthetic fleet trước rollout thật.

## 10. Senior/Architect judgment

Tách **safety** (không làm mất 3/4 inverter/site), **liveness** (tiến độ khi mạng hồi phục), **observability** (biết chắc trạng thái). SLA không thể mạnh hơn dữ liệu và khả năng điều khiển vật lý. Phương án đơn giản hơn vẫn hợp lệ nếu chứng minh invariants, residency, budget và recovery.

## References to consult after lab

Microsoft Learn về Azure IoT Device Update, distributed messaging, Well-Architected reliability; RFC 9110 HTTP semantics; NIST firmware security guidance. Kiểm tra khả năng phần cứng và phiên bản dịch vụ trước triển khai.
