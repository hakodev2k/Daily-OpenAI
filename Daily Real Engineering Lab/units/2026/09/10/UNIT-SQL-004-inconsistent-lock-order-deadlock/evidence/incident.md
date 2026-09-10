# Incident Evidence

## Business invariant

- Tổng quantity của hai location phải giữ nguyên sau mỗi operation thành công.
- Không location nào được âm.
- Hai operation độc lập có thể chạy đồng thời.

## Timeline

```text
22:01:14.210 Operation A begins: Location 1 -> Location 2
22:01:14.214 Operation B begins: Location 2 -> Location 1
22:01:14.221 A changes Location 1
22:01:14.223 B changes Location 2
22:01:15.227 A waits for Location 2
22:01:15.228 B waits for Location 1
22:01:15.231 Database resolves the circular wait by canceling one operation
22:01:15.236 remaining operation completes
```

## Simplified wait-for view

```text
A owns resource for Location 1 and requests Location 2
B owns resource for Location 2 and requests Location 1
```

## Operational observations

- CPU and storage latency remain near baseline.
- Connection pool remains healthy.
- Failure appears only under overlapping opposite-direction operations.
- Immediate retry often succeeds but does not remove the underlying concurrency condition.

## Constraints

- Peak workload is high enough that one global application lock is not acceptable.
- p95 latency matters.
- Upstream may retry requests.
- Team operates one SQL Server primary today.
- Small schema or application changes are allowed; a platform rewrite is not.
