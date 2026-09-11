# Incident Snapshot

Đây là **expected reproduction evidence**, không phải kết quả runtime đã được thực thi trong quá trình generate lab.

Khi starter còn nguyên, output phải có dạng tương đương:

```text
CLIENT_CANCELLATIONS_SENT=6
ACTIVE_PRODUCERS_AFTER_GRACE=>0
POOL_IN_USE_AFTER_GRACE=>0
WAITING_FOR_POOL_AFTER_GRACE=>=0
RESULT=INCIDENT_REPRODUCED
```

Điểm cần điều tra không phải giá trị timing tuyệt đối mà là invariant: sau khi tất cả client đã phát cancellation và qua grace period, producer vẫn còn hoạt động hoặc tài nguyên pool vẫn còn bị giữ/chờ.

Sau khi sửa đúng:

```text
CLIENT_CANCELLATIONS_SENT=6
ACTIVE_PRODUCERS_AFTER_GRACE=0
POOL_IN_USE_AFTER_GRACE=0
WAITING_FOR_POOL_AFTER_GRACE=0
RESULT=FIX_VERIFIED
```