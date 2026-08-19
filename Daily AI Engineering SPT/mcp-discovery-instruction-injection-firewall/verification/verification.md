# Verification

## Scope
This verification covers the reusable package itself: policy invariants, deterministic guard behavior, cache rules, approval rules, and artifact consistency. It does not claim that an external MCP client has already integrated the package.

## Implemented
- Deterministic MCP instruction validator.
- Configurable hard limits and suspicious/hard-block patterns.
- Explicit trust classes and taint output.
- Policy that forbids public caching for untrusted instructions.
- Sensitive-tool classification.
- Host-side approval requirements defined in rules/workflows.
- Regression test runner with benign and adversarial fixtures.
- Integration, recovery, and audit guidance.

## Measured
The package defines measurable controls:
- instruction classification coverage;
- allow/taint/block rates;
- untrusted public-cache hits;
- sensitive tainted calls;
- approval/denial counts;
- unauthorized sensitive executions;
- regression false positives/false negatives.

These runtime metrics require integration into a specific MCP host and are therefore not claimed as production measurements in this package.

## Verified package invariants
1. `config/policy.json` sets `publicCacheForUntrustedInstructions` to `false`.
2. The policy includes repository mutation and network egress among sensitive classes.
3. Audit defaults do not store raw instruction payloads.
4. `instruction_guard.py` uses strict UTF-8 decoding, Unicode normalization, byte/character limits, control-character checks, deterministic reason codes, SHA-256 hashing, and non-zero exit codes on blocks/errors.
5. The guard never emits normalized content for a `block` decision.
6. Remote/unknown trust classes produce taint.
7. Workflow rules explicitly prohibit model self-approval.
8. Cache workflow requires server identity, tenant, trust class, protocol version, policy version, and payload hash.
9. Tests cover benign content, override attempt, secret/exfiltration attempt, suspicious directive, managed source, oversized payload, control characters, and policy invariants.
10. Failure paths fail closed rather than reverting to unguarded instruction ingestion.

## Required runtime verification after integration
An integrating project must additionally prove:
- all MCP instruction ingestion paths invoke the guard before prompt assembly;
- no remote payload reaches system/developer authority;
- taint survives in host-managed metadata;
- sensitive tool calls under taint cannot execute without valid host authorization;
- argument changes invalidate prior approval;
- two tenants cannot share untrusted instruction cache entries;
- validator/policy upgrades force revalidation;
- regression tests pass in the project's supported runtime;
- audit logging does not expose secrets.

## Acceptance threshold
The reusable package is complete when all package artifacts exist and reference only real package files. A downstream integration is complete only after all runtime verification items above pass.

## Security status
No production safety claim is made without downstream integration evidence. The design intentionally distinguishes package implementation from production measurement and verification.