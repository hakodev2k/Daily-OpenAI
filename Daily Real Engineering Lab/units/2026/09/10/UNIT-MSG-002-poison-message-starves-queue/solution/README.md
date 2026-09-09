# Reference Solution — UNIT-MSG-002

> Chỉ xem sau khi đã reproduce và thử fix.

## Symptoms

Consumer liên tục nhận lại `msg-poison`; hai message hợp lệ phía sau không được xử lý trong observation window.

## Evidence

- `DeliveryCount` của cùng một message tăng liên tục.
- `Processed` vẫn bằng 0 trong starter.
- `Pending` vẫn bằng 3.
- `DeadLettered` vẫn bằng 0.

## Root cause

Failed message luôn được `Abandon` và đưa trở lại đầu ready queue, nhưng consumer không có terminal failure policy. Một poison message vì thế có thể chiếm vị trí đầu queue lặp lại vô hạn.

## Why the fix works

Reference solution giới hạn số delivery attempt. Khi message tiếp tục fail sau ngưỡng cho phép, nó rời ready queue sang dead-letter path. Consumer sau đó có thể tiếp tục xử lý các message hợp lệ.

## How to verify

Chạy:

```powershell
./verify.ps1
```

Expected summary:

```text
Processed: 2
DeadLettered: 1
Pending: 0
```

## Alternative fixes

- Broker-native max delivery count + dead-letter queue.
- Retry topic/queue với delayed retry khi failure có khả năng transient.
- Phân loại transient/permanent failure và chỉ retry transient failure.

## Wrong or misleading fixes

- **Tăng số vòng xử lý:** chỉ kéo dài thời gian trước khi symptom xuất hiện.
- **Chỉ requeue xuống cuối queue:** giúp các message khác tiến triển nhưng poison message vẫn có thể retry vô hạn và gây tải/chi phí.
- **Catch rồi Complete/Ack luôn:** làm mất message mà không có terminal failure evidence.
- **Restart worker:** reset symptom nhưng không sửa policy.

## Production implications

Broker production cần retry/dead-letter policy rõ ràng, monitoring cho dead-letter count, và runbook để inspect/replay message sau khi nguyên nhân được xử lý.

## Trade-offs

Ngưỡng retry quá thấp có thể dead-letter transient failure sớm; quá cao làm tăng latency, cost và queue pressure. Retry delay/backoff cũng cần phù hợp loại dependency và SLA.

## What a Senior engineer should notice

Failure handling là một phần của message lifecycle contract, không phải chỉ là `catch` exception. Cần thiết kế cả success path, retry path, terminal failure path, observability và replay strategy.
