# Decision brief

- Cutover ở t=60: east không nhận request nữa; DNS trả west.
- Trace có 2 request trước cutover, 5 request sau cutover.
- Contract: tất cả request sau cutover thành công, tối đa 5 connections; không sửa server, không tạo HttpClient mới mỗi request.
- So sánh (A) connection không giới hạn tuổi, (B) connection có tuổi tối đa, (C) HttpClient mới cho mỗi request.
- Nêu RTO, TLS handshake, socket pressure, retry safety, request in-flight, metric cần đo.
- Mô phỏng không đại diện cho DNS OS caching, real TCP, TLS hoặc load balancer; cần integration test thực tế trước rollout.
