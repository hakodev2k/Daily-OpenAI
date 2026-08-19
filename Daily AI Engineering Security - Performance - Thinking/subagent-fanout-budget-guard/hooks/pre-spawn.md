# Hook — Pre Spawn

## Trigger
Immediately before an orchestrator launches two or more subagents.

## Preconditions
`config/budget.json` is valid and parent-context/work estimates are available.

## Action
Run:
```bash
python scripts/fanout_budget.py check \
  --config config/budget.json \
  --parent-context-tokens "$PARENT_CONTEXT_TOKENS" \
  --agents "$AGENT_COUNT" \
  --expected-work-tokens "$EXPECTED_WORK_TOKENS" \
  --max-retries "$MAX_RETRIES" \
  --serial-baseline-tokens "$SERIAL_BASELINE_TOKENS"
```

Optionally pass `--tasks-json tasks.json` to detect exact normalized duplicate task descriptions.

## Expected result
Exit `0`: allowed or warning-level proposal within hard policy limits.

Exit `2`: hard budget violation or duplicate task; redesign before spawn.

## Failure behavior
Exit `3` means config/input error. Retry input collection once, then block large fan-out rather than assuming unlimited capacity.

## Blocking
Yes for exit `2` or `3`.