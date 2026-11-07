# After — behavior contract
- `.\verify.ps1` PASS với `VERIFIED` trên learner-edited starter.
- Timeline: `START dispatch → START gateway-send → STOP gateway-send → STOP dispatch`.
- `Business=True`, `TraceRelation=True`, `CorrectLifetime=True`, `OriginalSymptom=False`.
- `TraceId` trùng nhau; child `ParentSpanId` bằng parent `SpanId`. Không cần dựa vào mili-giây tuyệt đối.
