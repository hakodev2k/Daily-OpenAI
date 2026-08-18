# AI Production Incident Triage Agent

Reusable agent package for structured production incident investigation.

## Problem
Reduce unstructured debugging by forcing evidence collection, hypothesis tracking, bounded investigation, and verification.

## Workflow
Trigger -> Collect context -> Analyze evidence -> Form hypotheses -> Validate -> Recommend fix -> Verify

## Components
- skills: investigation procedures
- rules: safety boundaries
- subagents: specialized responsibilities
- workflows: execution lifecycle
- hooks/scripts: deterministic checks

## Safety
No production changes, data deletion, deployments, or configuration changes without approval.

## Definition of Done
- Evidence collected
- Root cause confidence documented
- Verification completed
- Remaining risks recorded
