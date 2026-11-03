# Incident evidence — local synthetic trace
Batch theo thứ tự input: P1(site-a,seq1), A2(site-a,seq2), B1(site-b,seq1), T1(site-c,seq1), C2(site-c,seq2).

Gateway fixture: P1 luôn trả permanent recipient rejection; T1 trả 503 ở lần gửi đầu, sau đó thành công; các message khác thành công.

Quan sát trên starter với 12 ticks:

```text
tick=0 retry=P1 attempt=1 error=PermanentGatewayException
tick=1 retry=P1 attempt=2 error=PermanentGatewayException
...
tick=11 retry=P1 attempt=12 error=PermanentGatewayException
delivered=[]
deadLetters=[]
P1-attempts=12
starter-symptom-reproduced=True
```

**Nhiễu có chủ đích:** gateway T1 có cấu hình transient failure nhưng chưa hề được gọi trong trace này. Đừng kết luận T1 là nguyên nhân sự cố hiện tại chỉ vì nó cũng có khả năng lỗi.
