# Synthetic incident evidence (cô lập, không phải log production)
- 10:24:00.000 — export A accepted, renderer active=1, MaxWorkers=1.
- 10:24:00.005 — export B queued, active=1.
- 10:24:00.012 — client disconnect for B; B finishes with OperationCanceledException.
- 10:24:00.013 — export C accepted and renderer active=2; A has not finished.
- 10:24:00.040 — export C finishes; A still processing.
- 10:24:00.090 — export A finishes.
- Dashboard noise: CPU 42%, free memory 63%, upstream HTTP success 100%, no pod restart.

Evidence chỉ cho thấy một invariant bị vi phạm sau cancellation. Những nguyên nhân có thể: cấu hình capacity bị thay đổi, telemetry đếm sai, lifecycle của request sai, hoặc resource accounting sai. Hãy xác minh bằng test và source, không suy luận từ CPU.
