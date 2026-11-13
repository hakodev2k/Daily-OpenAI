# Architecture Decision Record — learner workspace

## 1. Observed symptoms and baseline
- Attempts/s:
- Mean ingress MiB/s:
- Mean ingress Gbit/s:
- Max simultaneous 40 MiB buffers under 2 GiB headroom (upper bound):
- Important assumptions / uncertainties:

## 2. Hypotheses and disconfirming evidence
| Hypothesis | Supporting evidence | Falsification measurement |
|---|---|---|
| API | | |
| Blob | | |
| Auth | | |
| Client | | |

## 3. Alternatives
| Criterion | API proxy (streaming) | Browser direct-to-Blob staged | Resumable upload via dedicated gateway |
|---|---|---|---|
| Security | | | |
| Deadline correctness | | | |
| Failure recovery | | | |
| Cost / ops | | | |
| Complexity | | | |

## 4. Chosen design and explicit non-goals

## 5. Sequence: authorize -> upload -> finalize -> receipt

## 6. Trust boundary, upload token scope and expiration

## 7. Duplicate finalize, overwrite and deadline race policy

## 8. Virus scanning / quarantine / accepted state

## 9. Abandoned blobs, retention and cost

## 10. Observability, rollout, rollback

## 11. Failure matrix
| Failure | Expected behavior | Test |
|---|---|---|
| Network disconnect | | |
| Expired credential | | |
| Duplicate finalize | | |
| Wrong student/object | | |
| 41 MiB file | | |
| Scanner outage | | |
| Deadline passes mid-upload | | |

## 12. Trade-offs and decision reversal triggers
