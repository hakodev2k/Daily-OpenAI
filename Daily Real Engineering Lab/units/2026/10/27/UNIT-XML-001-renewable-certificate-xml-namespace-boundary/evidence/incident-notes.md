# Incident evidence — Batch OCT-27

Đây là dữ liệu synthetic, không phải log production thật.

```text
[08:15:01] PartnerUploadAccepted batch=OCT-27 bytes=348
[08:15:01] ImportCompleted batch=OCT-27 readingCount=0 totalKwh=0
[08:15:04] PartnerReconciliationMismatch batch=OCT-27 expectedMeterCount=2 expectedKwh=20
[08:16:20] OpsNote: retry of identical payload also produced zero readings
```

Bằng chứng sơ bộ: parser không throw, retry không làm thay đổi kết quả, trong payload có số đo cùng dữ liệu diagnostic. Đừng kết luận ngay rằng lỗi thuộc network, culture hay payload; hãy so sánh chính xác cấu trúc XML với bộ chọn dữ liệu và hợp đồng phiên bản.

Khi điều tra, ghi lại số lượng phần tử từng loại, tên đầy đủ của phần tử và vị trí của chúng trong cây XML. Không sử dụng số liệu chẩn đoán để đối soát.
