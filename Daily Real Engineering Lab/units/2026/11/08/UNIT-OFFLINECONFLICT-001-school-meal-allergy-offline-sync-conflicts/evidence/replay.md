# Evidence — Offline school meal replay

## Existing algorithm
Tablet lưu snapshot và cho phép sửa `allergyCleared` offline. Server merge bằng `clientUpdatedAt` mới nhất. Đồng hồ tablet không được kiểm soát. Sync delivery at-least-once.

| Event | UTC | Source | Student | Payload |
|---|---|---|---|---|
| E0 | 08:00 | Server | S104 | revision 40; allergyCleared=true |
| E1 | 08:05 | Tablet A | S104 | download revision 40, disconnect |
| E2 | 09:10 | Nurse / server | S104 | revision 41; allergyCleared=false, peanut restriction |
| E3 | 09:15 | Tablet A | S104 | offline edit allergyCleared=true; client clock says 12:15 |
| E4 | 10:00 | Tablet A | S104 | attempted meal scan while offline |
| E5 | 11:20 | Tablet A | S104 | upload E3 twice due to retry |
| E6 | 11:21 | Server | S104 | apply current merge rule |

## Constraints
3,000 students; 18 schools; 36 tablets; offline up to 6h. Online scan p95 < 400ms. Cloud cost < $450/month. Team 3 engineers, six-week rollout. Store health data minimally, encrypted and role-scoped.

**Safety invariant:** A newly recorded restriction cannot be silently removed by an older offline snapshot. If current safety status cannot be confirmed, hold serving or require documented supervised manual verification.

## Tasks
Calculate state after E6 under current rule. Evaluate clock skew, duplicate uploads, E4 offline behavior, conflict detection, audit trail and rollback.
