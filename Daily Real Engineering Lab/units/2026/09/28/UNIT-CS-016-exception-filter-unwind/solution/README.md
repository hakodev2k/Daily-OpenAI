# Reference Solution — inspect only after your attempt

## Symptoms

Filter telemetry sees `ActiveOperations=1`, while cleanup later leaves it at 0.

## Evidence

Starter timeline is `work → policy → cleanup → caught`.

## Root cause

C# exception filters are evaluated while the runtime searches for a handler, before stack unwinding executes the inner `finally`. Therefore a filter can observe state that cleanup has not normalized yet.

## Why the fix works

Do not make a filter decision depend on state whose invariant is restored by an inner `finally`. Catch the relevant exception and evaluate that state after unwind, or keep the filter limited to data that is valid during handler search.

## How to verify

Apply your change to `starter/Program.cs` and run `./verify.ps1`. The learner-editable path must no longer emit a cleanup-dependent `policy` observation before `cleanup`.

## Alternative fixes

A filter is still useful for exception type/data predicates that do not rely on cleanup. Another option is to capture immutable context before entering the scope and filter on that context.

## Wrong / misleading fixes

Adding delays does not alter exception semantics. Moving logging without moving the state-dependent decision only hides evidence. Removing `finally` avoids the observed ordering but breaks resource/state cleanup.

## Production implications

This matters when filters read ambient transaction state, counters, locks, tracing scope, or other state normalized during unwind. Side effects inside filters are especially difficult to reason about.

## Trade-offs

A broader `catch` followed by an explicit policy can be more verbose, but its ordering relative to cleanup is clearer. Filters remain concise when their predicates are pure and independent of unwind side effects.

## What a Senior engineer should notice

Source-code nesting does not fully describe runtime exception phases. Evidence should establish ordering before changing cleanup or handler policy.
