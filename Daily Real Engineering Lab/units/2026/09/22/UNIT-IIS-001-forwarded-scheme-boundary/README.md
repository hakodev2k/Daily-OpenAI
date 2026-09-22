# UNIT-IIS-001 — Forwarded Scheme Boundary

## Mục tiêu
Điều tra một redirect loop chỉ xuất hiện khi ASP.NET Core chạy sau IIS/reverse proxy có TLS termination.

## Bối cảnh thực tế
Customer Portal nhận HTTPS ở edge/IIS nhưng kết nối từ proxy đến Kestrel là HTTP. Sau release, browser nhận chuỗi redirect lặp lại dù certificate và public URL đều đúng.

## Nhiệm vụ
1. Chạy starter và reproduce symptom.
2. Dùng evidence để giải thích vì sao application luôn cho rằng request là HTTP.
3. Sửa request-pipeline boundary để redirect decision thấy đúng external scheme.
4. Không hard-code public URL và không tắt HTTPS redirection.
5. Chạy verify và ghi lại root cause/trade-off trong `workspace/my-investigation.md`.

## Chạy
- `./run.ps1`
- `./reproduce.ps1`
- Sau khi sửa: `./verify.ps1`

## Evidence cần quan sát
- `X-Forwarded-Proto=https` đại diện scheme phía client/proxy.
- Scheme mà application sử dụng tại thời điểm quyết định redirect.
- Thứ tự xử lý forwarded headers và HTTPS redirection.

## Hints
Mở lần lượt `hints/hint-01.md` → `hint-03.md` nếu cần.

## Reference solution
Chỉ mở `solution/README.md` sau khi tự hoàn thành.

## Expected result
`verify.ps1` phải xác nhận request forwarded HTTPS không bị redirect lại, trong khi request HTTP thực sự vẫn yêu cầu HTTPS.
