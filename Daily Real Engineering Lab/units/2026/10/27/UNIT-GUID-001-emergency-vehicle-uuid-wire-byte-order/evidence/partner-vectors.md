# Evidence — synthetic partner golden vectors

| Correlation | UUID trong partner registry | Expected 16-byte wire frame (hex) |
|---|---|---|
| EVT-A | 00112233-4455-6677-8899-aabbccddeeff | 00112233445566778899AABBCCDDEEFF |
| EVT-B | 12345678-9abc-def0-1122-334455667788 | 123456789ABCDEF01122334455667788 |

Telemetry: frame length = 16, TLS và transport thành công; local round-trip xanh nhưng partner lookup unknown vehicle. Đây là fixture synthetic, không phải dữ liệu production.
