# Current data flow — synthetic inventory

```text
Citizen -> ASP.NET Core API -> SQL Server (authoritative records)
                     |             |
                     |             +-> nightly export -> reindex worker -> Elasticsearch
                     +-> deletion event -> search worker -> Elasticsearch
Public browser -> Search API -> Redis query cache -> Elasticsearch
Public browser -> Detail API -> SQL Server
Public browser -> short-lived signed attachment URL -> Blob Storage
Backup jobs -> SQL / Blob snapshots (30-day retention)
```

## Ownership and boundaries

| Surface | Owner | Source | Refresh/expiry | Public exposure |
|---|---|---|---|---|
| SQL record | Records team | primary transaction | immediate | detail API |
| Elasticsearch | Search team | event + nightly export | eventually consistent | search API |
| Redis | Search team | query response | TTL 15 minutes | search API |
| Blob | Media team | upload pipeline | immutable object until cleanup | signed URL |
| Backup | Platform team | daily snapshot | 30 days | no direct public endpoint |
| Support status | Support automation | request API response | immediate | customer communication |

## Design caveats

- Search document IDs are tenant-scoped; the same record ID may exist in another tenant.
- An already issued signed URL can remain valid independently of an API session.
- Workers use at-least-once delivery; order is not guaranteed.
- Reindex exports can overlap live mutations.
- Backup restore can bring back older snapshots.

Hãy đánh dấu trust boundaries, authority boundaries và consistency boundaries trên sơ đồ. Không tự giả định rằng một cache invalidation hay một event ACK là đủ cho end-to-end correctness.
