# Senior challenge — Export compatibility decision

1. Thiết kế compatibility matrix cho Excel, LibreOffice và import wizard, không chạy untrusted formulas ngoài môi trường cô lập.
2. So sánh CSV text-prefix policy với XLSX explicit string cell types về security, cost, backward compatibility.
3. Test save/re-import round-trip và sự tồn tại của text prefix.
4. Đề xuất telemetry chỉ ghi số cell neutralized, không log PII.
5. Lập migration plan cho machine consumers nếu prefix làm thay đổi output contract.
