# Incident evidence — synthetic fixture

~~~text
02:10:00.000 INFO  intake.batch       batch=BL-427 expected=9
02:10:00.001 INFO  intake.ack         batch=BL-427 acknowledged=9
02:10:00.002 INFO  audit.flush        batch=BL-427 persisted=3 sequences=7,8,9
02:10:00.002 WARN  reconciliation     batch=BL-427 expected=9 persisted=3
02:10:00.003 INFO  host.resources     cpu=low heap=stable
02:10:00.004 DEBUG partner.heartbeat  status=200 latencyMs=13
~~~

Dữ liệu giả lập: heartbeat không chứng minh pipeline audit đúng; admission không đồng nghĩa durability. Hãy tự tạo evidence thực bằng scripts.
