# Incident timeline (synthetic)

| ICT | Evidence | Observation |
|---|---|---|
| 09:00 | Release R42 | Ingress code deployed |
| 09:05 | Partner A ticket | Valid v1 manifests unexpectedly rejected |
| 09:08 | Audit queue | Some v2 manifests missing hazard declaration accepted |
| 09:12 | Traffic | 35% v1, 65% v2; CPU 18%, memory stable |
| 09:16 | DB/queue | No downstream schema or capacity change |

Sanitized examples: M-101 v1 weightKg 12.5 rejected; M-104 v2 missing hazardClass accepted; M-105 unknown version accepted. TLS errors and database pool waits unchanged (noise).
