# Incident Evidence

## Before deployment

- request rate: stable
- checkout success rate: stable
- exporter memory: stable
- metric series for checkout latency: low tens

## After deployment

- request rate: still stable
- checkout success rate: unchanged
- CPU: no meaningful increase
- application heap: small increase only
- telemetry exporter memory: grows as more distinct users become active
- metric series count: grows from tens to thousands
- no application exception correlated with the growth

## Reproduction snapshot

With 2,000 synthetic requests and 1,500 distinct users:

- measurements remain 2,000
- business dimensions such as region/status have only a few combinations
- observed series count is roughly in the thousands

One piece of noise: the 500-response ratio is intentionally fixed at 5%; it is not the driver of series growth.
