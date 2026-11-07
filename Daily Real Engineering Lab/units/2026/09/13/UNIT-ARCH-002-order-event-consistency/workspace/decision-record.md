# My Architecture Decision

## Symptoms / Business Risk

## Failure Window

## Evidence Collected

## Assumptions

## Options Considered

### Option 1

### Option 2

### Option 3

## Selected Option

## Why This Option

## Transaction / Durability Boundary

## Duplicate Delivery Strategy

## Retry / Recovery Strategy

## Observability

## Operational Cost

## Trade-offs Accepted

## Verification / Failure-Path Checklist

- [ ] Process dies immediately after SQL commit.
- [ ] Broker is unavailable for 10 minutes.
- [ ] Publisher sends the same event twice.
- [ ] Consumer crashes after side effect but before acknowledgement.
- [ ] HTTP request is retried by the client.

## Remaining Questions
