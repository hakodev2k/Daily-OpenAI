# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms
Từ client HTTPS, edge terminate TLS rồi gọi Kestrel bằng HTTP. Backend trả 307 với `Location: https://partner.example.test/status`; client lặp lại cùng hành trình. Gọi HTTP trực tiếp cũng nhận 307 nhưng đây là hành vi bảo mật đúng.

## Evidence
Probe tắt auto-follow redirect, phân biệt `EDGE_SIM` (gửi `X-Forwarded-Proto: https` từ loopback giả lập trusted proxy) với `DIRECT_HTTP`. Không có HTTP 500; certificate tại edge không phải nơi phát sinh redirect. `evidence/incident-timeline.md` là dữ liệu giả lập, không phải kết quả chạy thực.

## Root cause
Starter gọi `UseHttpsRedirection()` trước `UseForwardedHeaders()`. Request vào Kestrel có scheme HTTP; redirect middleware quyết định 307 và short-circuit, nên forwarded headers không có cơ hội cập nhật `Request.Scheme`. Edge nhận redirect quay về URL HTTPS ban đầu rồi tiếp tục gửi request HTTP nội bộ.

## Why the fix works
Đặt `UseForwardedHeaders()` trước `UseHttpsRedirection()` để scheme của request từ proxy **đã được tin cậy** được áp dụng trước khi quyết định redirect. Proxy HTTPS hợp lệ trả 200, trong khi HTTP trực tiếp không có forwarded header vẫn bị 307. Xem [Program.cs](Program.cs).

## Verification
1. `reproduce.ps1` với starter nguyên bản: `EXPECTED_REPRODUCED`.
2. Chỉnh `starter/Program.cs` theo hypothesis; chạy `verify.ps1`: `VERIFIED`.
3. Nếu cần đối chiếu: `verify.ps1 -Reference` chạy solution riêng; tuyệt đối không thay cho bước 2.
4. Kiểm tra đồng thời cả 200/https tại edge giả lập và 307 của direct HTTP.

## Alternative fixes
- Chấm dứt redirect ở edge, chỉ nhận traffic từ ingress trong private network: có thể phù hợp nếu edge đảm bảo HTTPS và backend không public; cần ghi rõ security boundary.
- End-to-end TLS tới Kestrel: loại bỏ scheme mismatch nhưng tăng vận hành certificate/rotation; không phải mặc định tốt hơn.
- Điều chỉnh proxy/ingress để truyền `X-Forwarded-Proto` chuẩn: cần thiết nếu header thiếu, nhưng **không đủ** khi middleware xử lý sai thứ tự.

## Wrong / tempting fixes
- Tắt `UseHttpsRedirection` toàn bộ: che triệu chứng, có thể bỏ bảo vệ đường HTTP direct.
- Cho client retry thêm: lặp lại cùng redirect, tăng tải và thời gian lỗi.
- Đổi certificate hoặc xóa cache khi handshake TLS đã tốt: không tác động tới scheme tại Kestrel.
- `KnownNetworks.Clear(); KnownProxies.Clear()` mà không bổ sung allowlist: tin mọi client tự khai HTTPS, có thể làm bypass chính sách transport. Lab chỉ tin loopback để mô phỏng edge cục bộ.

## Production implications
Trong Kubernetes/NGINX/Azure ingress, proxy có thể không dùng 127.0.0.1; phải cấu hình `KnownProxies`/`KnownNetworks` theo IP/CIDR tin cậy, giới hạn số hop và xử lý proxy chain. Kiểm thử ingress thực với HTTP→HTTPS, HTTPS→HTTP upstream, host forwarding và proxy spoofing. Không copy allowlist loopback của lab vào production nếu topology khác.

## Trade-offs
Forwarded headers nhẹ, phù hợp TLS termination nhưng tạo dependency vào trusted-proxy contract. End-to-end TLS tăng bảo vệ đường nội bộ và chi phí certificate. Edge-only redirect có thể đơn giản hơn nếu đường direct hoàn toàn không public. Lựa chọn dựa vào threat model và topology, không chỉ dựa vào 200 OK.

## Senior engineer should notice
Một HTTP 307 có thể là lỗi kiến trúc tại ranh giới reverse proxy thay vì exception trong business code. Luôn tách scheme bên ngoài khỏi scheme mà ứng dụng quan sát, bảo toàn trust boundary và kiểm tra non-regression cho direct HTTP. Dùng logs/traces để chứng minh giả thuyết trước khi đổi policy.

## Static validation
Code được kiểm tra tính nhất quán tĩnh khi tạo lab; không tuyên bố đã build hoặc chạy trong môi trường tạo nội dung.
