# Evidence — exam deadline incident (synthetic)

| Measurement | Value | Notes |
|---|---:|---|
| Candidates | 12,000 | all eligible |
| Uploads in final 10 minutes | 7,200 | successful + retries counted as attempts |
| Peak 60-second window | 1,800 attempts | traffic shape is bursty |
| PDF median / p95 | 8 MiB / 24 MiB | hard max 40 MiB |
| Mean file size for planning | 12 MiB | explicitly a planning assumption |
| API inbound network budget | 1.2 Gbit/s | aggregate across replicas |
| API app memory | 8 GiB | aggregate memory headroom available for upload buffering: 2 GiB |
| Gateway request timeout | 60 s | enforced by upstream |
| Existing implementation | browser -> API -> Blob | API proxy path |
| Current API implementation | buffers complete file in memory before Blob write | behavior observed from code review |
| Blob throughput | unknown | must validate in staging; do not claim unlimited |
| Compliance | only authenticated enrolled students; 30-day retention; audit receipt | no public anonymous upload |
| Availability target | 99.9% during exam window | provisional SLO |
| Deadline semantics | accepted before 17:00 local | authoritative server timestamp, not client clock |

## Replay tasks
- Convert 1,800 uploads/minute to attempts/second.
- Estimate payload ingress for 12 MiB mean and compare to 1.2 Gbit/s (assume decimal Gbit and binary MiB; state conversion).
- Estimate 40 MiB buffered uploads that fit within 2 GiB headroom; account for overhead.
- Explain why mean rate alone is insufficient for p95 latency and deadline correctness.
- Identify missing measurements before production go-live.

## Competing hypotheses
A. API ingress/buffering capacity dominates.
B. Blob write throughput dominates.
C. Authentication/token minting dominates.
D. Client uplink bandwidth dominates.
Mark what evidence would distinguish each, and what would falsify it.
