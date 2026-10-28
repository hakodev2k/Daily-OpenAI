# Incident snapshot (synthetic, deterministic)

Client-side probe:
```text
GET /v1/bulletins/current   -> 200
GET /v1/bulletins/limited   -> 429
GET /v1/bulletins/partner   -> 503
GET /v1/bulletins/unknown   -> 404
```

Internal status metric:
```text
/v1/bulletins/current status=200 count=1
/v1/bulletins/limited status=200 count=1
/v1/bulletins/partner status=200 count=1
/v1/bulletins/unknown status=200 count=1
```

Operations notes (distractors; no measured causal link):
- Synthetic background-worker CPU 12–17% in same window.
- Partner gateway reported p95 latency 240 ms in a separate dashboard.
- No exceptions in these four deterministic request paths.

Investigation: which observation directly contradicts another, and what evidence would separate endpoint behavior from measurement behavior?
