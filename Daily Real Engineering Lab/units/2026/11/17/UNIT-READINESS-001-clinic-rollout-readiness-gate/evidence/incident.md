# Incident evidence (synthetic, deterministic)
- t=0: process started; appointment reference data not loaded.
- t=1–3: upstream receives intermittent 503 from new instance.
- t=4: reference data load completes; 200 responses resume.
- CPU 12–19%; memory 140–160 MiB; no OOM or crash.
- Existing health dashboard shows healthy from t=0.

Các dữ kiện trên là fixture mô phỏng, không phải telemetry thực tế. Hãy giải thích vì sao CPU và memory không phải chứng cứ kết luận.
