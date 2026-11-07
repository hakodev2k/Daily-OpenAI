# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Sequential rendering is correct, while two coordinated concurrent renders can mix statement content.

## 2. Evidence

The deterministic reproduction pauses render A after its first write, lets render B use the same formatter, then resumes A. A's returned text no longer represents only A.

## 3. Root cause

`LegacyStatementFormatter` stores a mutable `StringBuilder` in static state. Every invocation mutates the same buffer, so concurrent operations do not own independent intermediate state.

## 4. Why the fix works

Create the mutable buffer inside `Render`. Each invocation then owns its intermediate formatting state while the existing public method and output contract remain unchanged.

## 5. How to verify

Run `verify.ps1`. It checks both the sequential contract and a deterministic interleaving of two renders through the learner-editable `starter/` path.

## 6. Alternative fixes

A lock around the complete formatting operation can make the current implementation correct, but serializes all renders. An object-per-operation formatter can also work if ownership remains explicit.

## 7. Wrong or misleading fixes

Adding random delays only changes timing. Locking individual `AppendLine` calls does not make the whole multi-step render atomic. Replacing `StringBuilder` with another shared mutable collection preserves the ownership problem.

## 8. Production implications

Static mutable helpers often survive for years because sequential tests hide concurrency assumptions. Increasing worker parallelism or hosting the code in a concurrent server can expose the latent defect without any business-code change.

## 9. Trade-offs

Operation-local allocation is simple and naturally isolated, at the cost of one builder allocation per render. Pooling can reduce allocations but reintroduces lifecycle and reset obligations; use it only with measurement and strong ownership rules.

## 10. What a Senior engineer should notice

The important refactoring question is not merely 'is this type static?' but 'who owns mutable state, for how long, and can two operations access it concurrently?'. Preserve externally observable behavior while shrinking mutable-state lifetime to the narrowest useful boundary.