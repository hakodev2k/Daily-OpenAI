# After — starter đã sửa
- `./verify.ps1` exit 0, in `PASS: both API prefixes`.
- Prefix A: POST `/carrier/labels/v2/labels/submit` → 202; GET status → 200.
- Prefix B: POST `/regional/eu/carrier/labels/v2/labels/submit` → 202; GET status → 200.
- Body chứa shipmentId hợp lệ; không cần network.
- Nếu thất bại, đọc `Observed:` và so sánh với prefix; không sửa fake partner để ép test pass.
