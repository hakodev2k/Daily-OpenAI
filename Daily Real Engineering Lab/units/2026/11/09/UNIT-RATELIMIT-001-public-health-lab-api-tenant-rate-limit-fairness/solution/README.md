# Reference Solution — đọc sau khi tự làm

Đây là **một phương án có thể bảo vệ được**, không phải kiến trúc duy nhất.

## Baseline
W1: A30/B20/C20, tổng70. W2: B95/C5/A0, tổng100. W3: B100/C0/A0, tổng100. W4: B25/C20/A10, tổng55. W2 và W3 vi phạm mục tiêu khẩn cấp dù tổng throughput vẫn ở giới hạn quota.

## Quyết định
Chọn phương án C để rollout đầu tiên: global admission budget **120/10s**, reservation A=30, C=20, B base=70. Admission ưu tiên A rồi C rồi B trong cùng cửa sổ; B được mượn capacity **chỉ sau khi reservation của A/C đã được đáp ứng hoặc cửa sổ được đóng theo một policy có thể chứng minh**. Để tránh loan-before-arrival làm mất guarantee, phiên bản đầu **không cho B mượn reservation chưa sử dụng trong cửa sổ hiện tại**. Như vậy spare reserved capacity có thể bị lãng phí: trade-off được chấp nhận để bảo vệ A. Cho phép borrowing trong tương lai chỉ khi có preemption/queuing hoặc predictive admission có safety margin; không hứa guarantee khi đã cấp token không thể thu hồi.

Policy deterministic để replay: A tối đa 30, C tối đa 20, B tối đa 70 trong 10s; global <=120. Với bảng workload:
- W1: A30/B20/C20 =70.
- W2: A30/B70/C15 =115; B bị 429:25.
- W3: A30/B70/C20 =120; B bị 429:40.
- W4: A10/B25/C20 =55.

Không đánh đồng reservation với throughput cam kết khi dependency thực sự không sẵn sàng; capacity target chỉ hợp lệ trong phạm vi load-test assumptions.

## Vì sao không chọn ngay các phương án khác
- A: B có thể chiếm 120 trước A; tăng quota không giải quyết fairness.
- B: đơn giản, nhưng tổng base chỉ 100; phần nhàn rỗi không tái sử dụng.
- D: token bucket và dynamic borrowing cải thiện utilization, nhưng cần thêm state, chính sách burst, atomic cross-replica và test theo thứ tự arrival; phức tạp hơn yêu cầu rollout đầu tiên.

## Distributed coordination
Dùng shared atomic counter/limiter theo 10s window với Redis Lua script hoặc service quota coordinator; key gồm policy version, tenant class, window ID. Thao tác atomic kiểm tra cả per-class và global cap trong cùng transaction. TTL >= window + safety margin. Đồng hồ xác định window phải thống nhất (server-side time hoặc clock discipline), không để hai gateway tự chọn window lệch. Chặn request khi coordinator unavailable đối với bulk B; với A/C, chọn fail-closed có thông báo degraded và alert trong bản đầu để không vượt safety cap, rồi đánh giá local emergency budget có giới hạn với reconciliation trong phiên bản tiếp theo. Redis failover, time skew, partial partition và idempotent retries cần chaos test.

## HTTP & observability
429 có Retry-After phù hợp policy, structured reason code, no sensitive patient data trong metric labels. Đo accepted/rejected theo class, p95 admission latency, global utilization, limiter-store error, retry amplification và SLO urgent. Dùng canary 5%→25%→100%, so sánh workload W2/W3, rollback bằng policy version + feature flag, không reset quota trong giữa window khi đổi version.

## Wrong fixes
- Chỉ tăng replica: không sửa quota fairness và có thể tăng tải downstream.
- Chỉ đổi global limit: tenant đến trước vẫn chiếm capacity.
- Counter riêng trên mỗi gateway: quota hiệu dụng bị nhân đôi.
- Cho B mượn ngay 30 reserved của A khi A chưa đến: không thể đảm bảo A sau đó nhận 30.
- Retry ngay không jitter: 429 khuếch đại thành burst.

## Verification
Tính lại 4 cửa sổ với cả thứ tự B→C→A và A→C→B; kết quả với per-class cap phải bất biến. Test 2 gateway concurrent: accepted không vượt 120 và A vẫn được nhận 30 nếu đủ backend capacity. Test coordinator outage, failover và window boundary. Đây là static-review tabletop, chưa chạy load test thực tế.
