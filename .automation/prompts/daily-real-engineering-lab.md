# Daily Real Engineering Lab — Runtime Prompt

Generate and maintain the user's **Real Engineering Lab** learning program for this run.

Do not create, modify, explain, or suggest an automation or schedule.
Do not turn this workflow into an interview-question generator.
Every scheduled run MUST generate and save exactly one new lab. Never skip lab generation because previous labs are unfinished.
Do not ask follow-up questions before executing the run.

Use the actual current Vietnam time:

`Asia/Ho_Chi_Minh / UTC+7`

All editorial learning content must be written in **Vietnamese**, while preserving technical terms, APIs, framework names, commands, code, error messages, protocol names, Azure service names, database terminology, CVE identifiers and product names in their original language when that is more precise.

The primary deliverable is the GitHub lab content. Do not paste the full lab into chat.

This scheduled job is expected to run approximately **once per hour**. Therefore the prompt must optimize for:

- high-frequency generation
- anti-duplication
- idempotent retries
- compact persistent state
- scalable repository organization
- long-term diversity
- bounded context usage
- maintainable shared lab infrastructure

A one-hour cadence must never cause repeated labs, duplicated retry output, uncontrolled state growth or misleading learning progress.

---

# 1. Objective

Build a long-running, adaptive, hands-on engineering curriculum and a **long-term Real Engineering Lab library** for a .NET-focused developer with approximately **3–4 years of real hands-on experience** whose CV contains broad technology exposure but whose main growth goal is to deepen root-cause understanding, engineering judgment and production reasoning.

The scheduled job has two parallel goals:

1. continuously generate a diverse, reusable engineering-lab library that becomes progressively more challenging over time
2. preserve user mastery separately so library growth never falsely implies learner mastery

The library is intentionally allowed to grow faster than the learner can complete it.

The target progression is:

**Middle Developer → Senior .NET Developer → Technical Lead → Software / Solution Architect**

The system must convert technology exposure into actual engineering mastery through:

**reproduce → observe → collect evidence → hypothesize → investigate → fix → verify → compare → understand root cause → evaluate trade-offs → revisit later**

The learning system must prioritize practical engineering behavior over memorized definitions.

The user should repeatedly experience real technical symptoms, run code locally, inspect evidence, form hypotheses, implement fixes, verify results and compare with high-quality reference solutions.

The system must train the user to become better at:

- C# and .NET runtime reasoning
- ASP.NET Core
- Entity Framework Core
- SQL and database engineering
- asynchronous programming and concurrency
- HTTP and networking
- API engineering
- caching
- messaging and distributed systems
- Azure application architecture
- production troubleshooting
- performance engineering
- observability
- security
- testing
- DevOps and deployment
- legacy modernization
- software design
- architecture decisions
- technical trade-offs
- system design
- technical leadership
- solution architecture

---

# 2. Core Learning Philosophy

This is a **lab-first learning system**.

The main learning unit must NOT be:

- a list of interview questions
- a quiz
- a theoretical tutorial
- a collection of definitions
- a generic coding kata
- a LeetCode problem
- a random “bug of the day”

The primary learning experience must be a **reproducible engineering problem**.

A strong lab follows this mental loop:

```text
REAL ENGINEERING PROBLEM
        ↓
RUN STARTER SYSTEM
        ↓
REPRODUCE SYMPTOM
        ↓
OBSERVE BEHAVIOR
        ↓
COLLECT EVIDENCE
        ↓
FORM HYPOTHESES
        ↓
INVESTIGATE
        ↓
USE HINTS ONLY IF NEEDED
        ↓
IMPLEMENT FIX / DECISION
        ↓
VERIFY
        ↓
COMPARE REFERENCE SOLUTION(S)
        ↓
STUDY WRONG FIXES
        ↓
UNDERSTAND ROOT CAUSE
        ↓
UNDERSTAND TRADE-OFFS
        ↓
FUTURE REGRESSION / REVIEW
```

Questions may be used sparingly inside a lab to guide investigation, but pure Q&A must remain secondary.

The existing **Daily Interview & Code Review Challenge** already covers assessment behavior. Do not duplicate that job.

---

# 3. Evidence Over CV Claims

Treat technologies listed in the candidate profile as **exposure evidence**, not proof of mastery.

A technology may be present in the CV while the underlying mechanism, failure modes or trade-offs remain weak.

Use this hierarchy:

1. actual learning evidence from previous Real Engineering Labs
2. structured skill state
3. recorded knowledge gaps
4. prior lab outcomes
5. candidate project exposure
6. CV technology list

Never assume mastery only because a technology appears in the CV.

---

# 4. Candidate Profile

Use this profile only as the initial curriculum inventory and experience context.

## Core backend

- C#
- .NET
- .NET 6
- .NET 8
- .NET 10
- ASP.NET Core
- ASP.NET MVC
- ASP.NET Web API
- Entity Framework Core
- LINQ
- RESTful APIs

## Databases and storage

- SQL
- SQL Server
- PostgreSQL
- MongoDB
- Cosmos DB
- Redis
- Elasticsearch
- Azure Blob Storage
- query optimization
- index optimization
- caching

## Azure

- Azure App Service
- Azure Functions
- Azure Service Bus
- Azure Blob Storage
- Azure Monitor
- Application Insights
- Azure AD / Microsoft Entra ID concepts
- Azure AD B2C
- Managed Identity
- Azure Key Vault

## Architecture and backend patterns

- Clean Architecture
- Onion Architecture
- Domain-Driven Design
- CQRS
- MediatR
- Repository Pattern
- Unit of Work
- SOLID
- Design Patterns
- dependency injection
- asynchronous programming
- BackgroundService
- HostedService
- scheduled jobs
- third-party API integrations

## Frontend / full-stack exposure

- JavaScript
- TypeScript
- Vue.js
- React.js
- Angular
- jQuery
- HTML5
- CSS3

## CMS / commerce / web

- Optimizely CMS
- CMS architecture
- composable commerce
- e-commerce integrations
- SEO engineering
- Akamai / CDN concepts
- Google Lighthouse
- web performance

## DevOps / hosting

- Git
- Azure Repos
- CI/CD
- TeamCity
- Jenkins
- Octopus Deploy
- IIS
- Windows Server
- Azure App Service on Linux
- Kubernetes

## Quality / testing / operations

- Playwright
- unit testing concepts
- integration testing
- E2E testing
- SonarQube
- Azure Monitor
- Application Insights
- Kibana
- Elasticsearch logs
- production support
- production troubleshooting

---

# 5. Initial Priority

Unless existing learning state shows otherwise, prioritize foundational depth before advanced architecture.

Initial priority order:

1. C# semantics and .NET runtime
2. Task / Thread / ThreadPool / async / await / cancellation
3. LINQ / IEnumerable / IQueryable / expression trees
4. EF Core query behavior and change tracking
5. SQL fundamentals, indexes, execution plans, transactions and concurrency
6. HTTP / TCP / TLS / connection pooling / timeouts / retries
7. ASP.NET Core request pipeline and dependency injection
8. testing and failure-path testing
9. caching and Redis
10. messaging and idempotency
11. distributed systems
12. observability and production diagnostics
13. Azure architecture and cloud operations
14. containers / CI/CD / infrastructure concepts
15. system design
16. architecture decisions
17. technical leadership and solution architecture

Do not jump to sophisticated architecture merely because it sounds senior.

Foundation gaps may block advanced units.

---

# 6. Persistent State and High-Frequency Indexes

Repository:

`hakodev2k/Daily-OpenAI`

Use this root directory:

`Daily Real Engineering Lab`

Recommended structure:

```text
Daily Real Engineering Lab/
├── README.md
├── shared/
│   └── EngineeringLabKit/
├── state/
│   ├── learning-state.json
│   ├── skill-matrix.json
│   ├── knowledge-gaps.json
│   ├── review-queue.json
│   ├── recent-labs.json
│   ├── generation-ledger.jsonl
│   └── learning-events.jsonl
├── catalog/
│   └── YYYY/
│       └── MM/
│           └── YYYY-MM-DD.md
└── units/
    └── YYYY/
        └── MM/
            └── DD/
                └── UNIT-*/
```

Treat `state/learning-state.json` as authoritative for current global state.

Use `state/recent-labs.json` as the compact anti-repetition index.

Use `state/generation-ledger.jsonl` as the append-only full generation history.

Do NOT scan every historical unit on every hourly run.

When enough history exists, `recent-labs.json` should normally retain only the most relevant recent metadata needed for anti-repetition, preferably around **50–100 recent labs**.

The long-term history remains in `generation-ledger.jsonl` and individual unit metadata.

If the directory or state files do not exist, initialize them before generating the first lab.

Do not infer completion from:

- calendar age
- chat history
- file creation date
- number of scheduled runs
- Git commits alone

---

# 7. Hourly Run Slot Idempotency

Because the scheduled job runs approximately once per hour, retries or duplicate executions may occur.

Define a Vietnam-time hourly slot:

```text
RUN_SLOT = YYYY-MM-DD-HH
Timezone = Asia/Ho_Chi_Minh
```

Example format only:

`2026-09-07-15`

The example must never be reused as a real slot.

Before generating a lab:

1. obtain the actual current Vietnam time
2. calculate `RUN_SLOT`
3. check whether `generation-ledger.jsonl` or current state already records a successful lab for that exact slot

If the same `RUN_SLOT` already has a successfully saved lab:

- do NOT generate another lab for that slot
- return the existing lab path
- treat this as idempotent retry handling, not as skipping a scheduled generation

This is the only normal case where an execution may avoid creating a new lab.

A later hourly slot must still generate a new lab.

---

# 8. Always Generate a New Lab

Every run MUST generate exactly **one new Real Engineering Lab**.

This rule is unconditional with respect to previous completion state.

Previous labs may be:

- `READY`
- `WAITING_FOR_USER`
- `IN_PROGRESS`
- `NEEDS_REWORK`
- `PAUSED`
- `STALE`
- `REVIEW_REQUIRED`
- `COMPLETED`
- `MASTERED`
- `SKIPPED`
- `ABANDONED`

None of these states may prevent creation of the current run's new lab.

The scheduled job is a **lab generation pipeline**, not a single-active-lesson workflow.

However, generating a new lab does NOT mean older labs are completed. Preserve their individual state exactly.

The system must therefore distinguish:

- generated labs
- attempted labs
- completed labs
- mastered labs

Never inflate learning progress merely because the scheduled job generated more content.

## Mandatory historical inspection before generation

Before selecting the new lab, inspect enough previous Real Engineering Lab history to avoid repetition and improve progression.

At minimum consider, when available:

- recent unit metadata
- unit titles
- domain
- lab format
- scenario
- primary skill
- secondary skills
- root cause
- failure mode
- solution pattern
- maturity level
- skill matrix
- knowledge gaps
- review history

Use old state and history to make the new lab **different, deeper, broader or more advanced**.

## Strict non-duplication rule

The new lab must NOT duplicate an old lab in substance.

Do not generate:

- the same scenario with renamed classes/entities
- the same root cause with cosmetic changes
- the same failure mode repeatedly
- the same solution pattern repeatedly
- the same lab format too close together
- the same business context too close together
- the same combination of technology + failure + fix too close together

Example of unacceptable repetition:

```text
Previous:
ASP.NET Core API slow because .Result blocks async I/O.

New:
Order API slow because .Wait() blocks async I/O.
```

This is essentially the same lab and must be rejected.

A repeated skill is allowed only when the new lab increases depth or changes the engineering dimension.

Example of valid progression:

```text
Earlier:
sync-over-async in one ASP.NET Core endpoint

Later:
ThreadPool starvation across multiple dependencies under load,
with traces, connection-pool evidence and competing hypotheses

Later:
architecture decision involving CPU-bound vs I/O-bound workloads,
bounded concurrency and service capacity
```

## Diversity window

Use recent history to create spacing between similar labs.

As a default:

- avoid the same primary root cause within roughly the most recent 10 generated labs
- avoid the same lab format within the most recent 3 labs when reasonable
- avoid the same business scenario/context within the most recent 5 labs
- avoid the same primary skill in back-to-back runs unless deliberate depth progression strongly justifies it

These are diversity defaults, not rigid curriculum limits.

If revisiting a skill, explicitly increase one or more of:

- mechanism depth
- ambiguity
- evidence complexity
- number of plausible hypotheses
- production realism
- scale
- concurrency
- failure interactions
- security constraints
- operational constraints
- architecture trade-offs
- business constraints

The next lab must add learning value beyond previous labs.

---

# 9. Learning Unit Identity

Use stable human-readable IDs.

Preferred format:

`UNIT-{DOMAIN}-{NNN}`

Examples:

- `UNIT-CS-001`
- `UNIT-DOTNET-004`
- `UNIT-EF-003`
- `UNIT-SQL-006`
- `UNIT-HTTP-002`
- `UNIT-AZURE-005`
- `UNIT-DIST-004`
- `UNIT-ARCH-003`

Determine the next sequence by inspecting existing units in that domain.

Never reuse a unit ID.

Unit folder:

`Daily Real Engineering Lab/units/YYYY/MM/DD/{UNIT_ID}-{slug}/`

---

# 10. Novelty Fingerprint

Every generated lab must contain a novelty fingerprint in `metadata.json`.

Recommended shape:

```json
{
  "novelty": {
    "businessContext": "ecommerce-checkout",
    "scenarioType": "dependency-degradation",
    "primaryRootCause": "connection-pool-exhaustion",
    "failureMode": "latency-spike",
    "solutionPattern": "bounded-concurrency",
    "labFormat": "production-incident"
  }
}
```

The exact labels may vary, but the metadata must capture enough structure to compare labs semantically.

Before publishing a candidate lab, compare its fingerprint with recent history.

Reject the candidate when it is materially too similar to recent labs.

Similarity must be evaluated across combinations, not just titles.

For example:

```text
User API + .Result + ThreadPool starvation
Order API + .Wait() + ThreadPool starvation
Product API + GetAwaiter().GetResult() + ThreadPool starvation
```

These are effectively the same lab and must not be published close together.

---

# 11. High-Frequency Diversity Windows

Because generation occurs roughly 24 times per day, use stronger spacing defaults.

Unless deliberate progression justifies an exception:

- avoid the same primary root cause within roughly the **24 most recent labs**
- avoid the same primary skill in consecutive runs
- normally separate the same primary skill by roughly **4–6 labs**
- avoid the same lab format within the most recent **3 labs**
- avoid the same business context within roughly the most recent **6–8 labs**
- avoid the same solution pattern within roughly the most recent **10–12 labs**

These are diversity defaults, not rigid curriculum laws.

A repeated skill is acceptable only when the engineering dimension changes materially.

Examples of valid progression:

```text
Redis:
stale cache
→ hot key
→ outage and graceful degradation
→ cache stampede
→ whether Redis should exist at all
```

Examples of invalid progression:

```text
SQL Orders query missing composite index
→ Products query missing composite index
→ Users query missing composite index
```

When revisiting a skill, increase one or more of:

- ambiguity
- mechanism depth
- evidence complexity
- number of hypotheses
- scale
- concurrency
- failure interactions
- operational constraints
- architecture trade-offs
- business constraints

---

# 12. Lab Maturity Levels

Use multiple lab formats. Do not make every lab a single obvious bug.

## L1 — Deterministic Bug Lab

Purpose:

- build foundational mental models
- reproduce one controlled problem
- learn one root cause deeply

Characteristics:

- clear local reproduction
- limited moving parts
- hints available
- deterministic failure whenever reasonably possible

Examples:

- sync-over-async
- EF Core materialization too early
- incorrect DI lifetime
- N+1
- incorrect index design
- cancellation not propagated

## L2 — Investigation Lab

Purpose:

- investigate symptoms using evidence
- distinguish several possible causes

Characteristics:

- problem is reproducible
- root cause is not directly marked
- evidence guides investigation
- limited noise may exist

## L3 — Multi-Cause Production Incident

Purpose:

- production troubleshooting
- hypothesis-driven debugging

Characteristics:

- incomplete information
- multiple plausible hypotheses
- logs / metrics / traces / SQL evidence
- some evidence is irrelevant noise
- user must prioritize investigation

## L4 — Refactoring / Modernization Lab

Purpose:

- evolve existing code safely

Examples:

- .NET modernization
- legacy async conversion
- API versioning
- testability improvement
- breaking dependency upgrade
- large service decomposition
- EF query modernization

## L5 — Design Decision Lab

Purpose:

- engineering judgment

There may be no bug.

Examples:

- Redis vs no cache
- direct DbContext vs Repository abstraction
- REST vs Service Bus
- SQL vs Cosmos DB
- monolith scaling vs microservices
- background worker vs Azure Functions

Provide multiple defensible options and trade-offs.

## L6 — Architecture / Solution Lab

Purpose:

- Technical Lead / Architect development

Include realistic constraints such as:

- throughput
- availability
- RPO / RTO
- security
- compliance
- cost ceiling
- team size
- operational capability
- migration constraints
- timeline
- regional requirements

The reference answer must be a defensible architecture, not “the one correct architecture”.

---

# 13. Calendar-Driven Difficulty Progression

The primary difficulty progression for newly generated labs must be driven by **calendar time since the Real Engineering Lab program started**, not by the number of labs the learner has completed.

Reason:

- this job runs approximately once per hour
- the learner may complete only a small subset of generated labs
- the repository is intended to become a long-term engineering lab library
- generated library difficulty should continue maturing even when the learner is temporarily inactive

Therefore maintain two independent concepts:

## Library Difficulty

Controls the difficulty of newly generated labs.

Library Difficulty increases gradually according to elapsed days/weeks/months since program start.

## User Mastery

Represents the learner's demonstrated capability.

User Mastery changes only from actual learning evidence such as:

- attempted labs
- completed labs
- investigation quality
- root-cause understanding
- verification quality
- trade-off reasoning
- production reasoning

Never increase User Mastery because:

- time passed
- more labs were generated
- the scheduled job ran many times

Library Difficulty and User Mastery must remain independent.

Example:

```text
Library:
Month 6
Typical generated difficulty = D4–D5

User mastery:
SQL = D2
Async = D3
Azure = D1
```

This is valid.

The learner may return to easier historical labs while the generator continues building harder future material.

## Difficulty Scale

Use a practical 7-band scale for generated difficulty:

- D1 — Guided Foundation
- D2 — Independent Foundation
- D3 — Investigation
- D4 — Production Troubleshooting
- D5 — Senior Engineering
- D6 — Technical Decision
- D7 — Architecture / Solution

Difficulty should increase through dimensions such as:

- fewer hints
- less explicit symptoms
- more code and components
- more plausible hypotheses
- noisier evidence
- concurrency
- scale
- partial failures
- distributed interactions
- security constraints
- operational constraints
- cost constraints
- business ambiguity
- multiple valid solutions
- architecture trade-offs

Do not equate difficulty only with code complexity.

## Suggested Calendar Progression

Use a gradual progression similar to:

```text
Weeks 1–2:
primarily D1–D2

Weeks 3–4:
primarily D2–D3

Month 2:
mix D2–D4

Months 3–4:
primarily D3–D4

Months 5–6:
mix D3–D5

Months 7–9:
primarily D4–D5

Months 10–12:
mix D4–D6

After 12 months:
primarily D5–D7 with continued lower-level regression labs
```

This is a progression guideline, not a rigid calendar ladder.

Do not jump directly from foundation to architecture merely because a date threshold was crossed.

## Daily Difficulty Band

Each calendar day should have a target difficulty band.

Example:

```text
Daily band:
D2–D3
```

Across that day's hourly labs, prefer a mix roughly like:

- ~20% easier reinforcement
- ~60% current-band labs
- ~20% stretch labs one level above the main band

The exact mix may vary to preserve curriculum diversity.

Do not increase difficulty every hour.

Difficulty progression happens at the **day / week / month level**.

## Foundation Preservation

As library difficulty rises, do not eliminate lower-level labs.

Continue generating periodic foundational and regression labs.

A mature library should contain:

- foundation refreshers
- intermediate investigations
- production incidents
- senior design decisions
- architecture / solution exercises

Later foundation labs should still become richer and more realistic rather than repeating beginner examples.

---

# 14. Rolling Difficulty Mix

Do not monotonically increase difficulty with every hourly run.

A high-frequency generator must keep a healthy mix of foundational and advanced labs.

Across a rolling window of approximately 24 generated labs, use a balanced distribution similar to:

- L1 Foundation / deterministic: ~6
- L2 Investigation: ~6
- L3 Production incident: ~5
- L4 Refactoring / modernization: ~3
- L5 Design decision: ~3
- L6 Architecture / solution: ~1

This distribution may adapt to demonstrated skill state, but do not let the system drift into:

- all beginner labs
- all architecture labs
- one domain dominating the day

The rolling window should expose a mix of:

- runtime
- framework
- database
- networking
- distributed systems
- cloud
- testing
- security
- observability
- performance
- deployment
- design
- architecture

---

# 15. Adaptive Difficulty

Difficulty must follow demonstrated mastery.

Typical progression:

```text
clear bug
→ hidden mechanism
→ debugging
→ production failure
→ multiple hypotheses
→ trade-off
→ design decision
→ architecture
```

If the user repeatedly struggles with a prerequisite:

- record the gap
- block the dependent advanced unit when necessary
- create a smaller repair unit
- resume the original progression later

Do not punish the learner with harder content merely because time passed.

---

# 16. Required Unit Structure

Every executable engineering unit should use the following structure where relevant:

```text
UNIT-*/
├── README.md
├── metadata.json
│
├── starter/
│   └── runnable source code
│
├── scripts/
│   ├── setup.ps1
│   ├── run.ps1
│   ├── reproduce.ps1
│   ├── verify.ps1
│   └── reset.ps1          # when useful
│
├── docs/
│   ├── prerequisite.md
│   ├── scenario.md
│   ├── investigation-guide.md
│   ├── concepts-after-lab.md
│   └── references.md
│
├── evidence/
│   └── logs / metrics / traces / SQL / timelines when relevant
│
├── hints/
│   ├── hint-01.md
│   ├── hint-02.md
│   └── hint-03.md
│
├── expected-results/
│   ├── before.md
│   └── after.md
│
├── workspace/
│   └── my-investigation.md
│
├── solution/
│   ├── README.md
│   └── reference source code
│
└── extensions/
    └── senior-challenge.md   # when useful
```

Not every unit needs every file, but executable units must contain enough material to satisfy the Reproducibility Contract.

Because this job runs hourly, **do not generate boilerplate files that add no learning value**.

Prefer a compact L1 unit such as:

```text
UNIT-*/
├── README.md
├── metadata.json
├── starter/
├── solution/
├── run.ps1
└── verify.ps1
```

Only add:

- `evidence/`
- `hints/`
- `docs/`
- `expected-results/`
- `workspace/`
- `extensions/`

when they materially improve that specific lab.

Avoid repository growth caused by empty or repetitive scaffolding.

---

# 17. Reproducibility Contract

Do not publish an executable lab unless the learner can reasonably answer all five questions:

1. **How do I run it?**
2. **How do I trigger the problem?**
3. **What should I observe?**
4. **Where should I inspect evidence?**
5. **How do I know I fixed it?**

The lab must provide concrete commands.

Prefer PowerShell commands because the learner commonly works on Windows.

Provide `.sh` equivalents when inexpensive and useful.

Avoid long environment setup.

Target:

**clone / pull → setup → reproduce**

in a small number of commands.

---

# 18. Shared EngineeringLabKit

High-frequency generation must avoid re-implementing common simulators in every unit.

Prefer reusable infrastructure under:

`Daily Real Engineering Lab/shared/EngineeringLabKit/`

Possible reusable components:

- Fake HTTP server
- Failure injection
- deterministic latency simulator
- retry/error simulator
- lightweight load generator
- fake message broker
- test-data generator
- metrics collector
- common assertions

Example conceptual APIs:

```csharp
failureInjector.FailEveryNthCall(5);
failureInjector.InjectLatency(TimeSpan.FromSeconds(2));
fakeHttp.ReturnStatusEvery(3, 503);
messageBroker.DuplicateNextMessage();
```

A new lab should reference or extend shared infrastructure when doing so reduces duplication.

Do not modify shared infrastructure recklessly on every run.

Only update shared components when:

- the change is backward compatible where practical
- multiple labs can reuse it
- the new behavior is clearly valuable

If modifying shared infrastructure could break older labs, prefer a unit-local helper instead.

---

# 19. Local-First Rule

Core labs should be runnable locally without requiring paid external services.

Prefer:

- local processes
- fake downstream APIs
- deterministic simulators
- Docker Compose
- local databases
- self-contained test data
- lightweight load generators

For Azure-specific topics:

- create a local simulation for the core mechanism when possible
- optionally provide a real Azure extension
- do not make an Azure subscription mandatory unless the topic fundamentally requires it

For external HTTP dependencies, provide a fake service with behaviors such as:

- fast response
- delayed response
- timeout
- 429
- 500 / 503
- intermittent failures
- malformed payload

For messaging topics, provide local simulation when possible for:

- duplicate delivery
- retry
- consumer crash
- redelivery
- poison messages
- ordering changes

---

# 20. Dependency Reproducibility

Pin versions where practical.

For .NET labs, prefer:

- `global.json`
- explicit TargetFramework
- pinned or centrally managed NuGet package versions

When Docker is used, pin image versions.

When Node tooling is used, pin relevant runtime/package-manager versions.

The lab should remain reproducible months later.

Do not use vague `latest` dependency versions unless there is a compelling reason.

---

# 21. Failure Injection

Create reusable or unit-local simulation mechanisms when useful.

Possible helpers:

- `Delay`
- `Timeout`
- `FailEveryNthCall`
- `RandomFailure` with deterministic seed
- `DuplicateMessage`
- `DropMessage`
- `ReorderMessage`
- `CrashAfter`
- `Throttle`
- `InjectLatency`

Prefer deterministic failure modes for foundational labs.

Randomness must not make the exercise impossible to reproduce.

---

# 22. Scenario Design

Every lab must begin with a realistic engineering context.

Good scenario sources include:

- REST APIs
- employee platforms
- event systems
- e-commerce
- CMS
- mobile backends
- scheduled jobs
- background workers
- search systems
- third-party integrations
- Azure-hosted applications
- internal business systems

The scenario should describe symptoms and business impact without immediately revealing the root cause.

Bad:

> `.Result` is blocking the thread. Fix it.

Good:

> The API works normally at low concurrency, but p95 latency grows disproportionately under load while CPU remains moderate. Investigate the request path and determine the bottleneck.

---

# 23. Progressive Hints

Hints must not spoil the answer too early.

Use progressive disclosure.

## Hint 0

No hint.

## Hint 1

Investigation direction.

Example:

> Classify the slow operation as CPU-bound or I/O-bound.

## Hint 2

Subsystem direction.

Example:

> Inspect what the request thread is doing while the downstream operation is incomplete.

## Hint 3

Concept direction.

Example:

> Look for synchronous waiting on `Task`.

Small comments may exist in starter code, but they must guide investigation rather than reveal the exact fix.

Avoid comments such as:

```csharp
// BUG: .Result blocks the request thread. Replace with await.
```

Prefer:

```csharp
// Investigation note:
// Does this request thread need to remain occupied
// while the downstream operation is incomplete?
```

---

# 24. Evidence-Driven Investigation

Higher-level labs must provide evidence rather than a highlighted buggy line.

Useful evidence may include:

- application logs
- structured logs
- exception traces
- request traces
- dependency telemetry
- latency measurements
- p50 / p95 / p99
- CPU
- memory
- GC data
- ThreadPool information
- SQL execution plans
- logical reads
- query timings
- HTTP status history
- incident timelines
- queue depth
- retry counts
- dead-letter information

At higher difficulty, include some irrelevant evidence so the learner must separate signal from noise.

Teach:

**symptom → evidence → hypothesis → verification → root cause → fix**

---

# 25. Investigation Workspace

Every investigation-oriented lab should include:

`workspace/my-investigation.md`

Template:

```markdown
# My Investigation

## Symptoms

## Reproduction

## Evidence Collected

## Hypotheses

1.
2.
3.

## Most Likely Root Cause

## Proposed Fix

## Expected Result

## Verification Result

## What I Learned

## Remaining Questions
```

The learner should be able to record reasoning before viewing the reference solution.

---

# 26. Expected Results

Provide expected behavior for both starter and fixed states.

Do not require exact timing values across different machines.

Use ranges, trends or qualitative conditions.

Example:

```text
Before:
- low concurrency appears normal
- p95 latency grows disproportionately at higher concurrency
- CPU does not necessarily reach 100%

After:
- throughput improves significantly
- p95 remains much more stable
- functional behavior remains correct
```

Also provide troubleshooting steps if the learner cannot reproduce the intended behavior.

---

# 27. Reference Solution

The learner explicitly wants a reference solution for self-comparison.

Every lab must provide a reference solution unless the lab is a design/architecture exercise where multiple answers are intentionally valid.

The solution must be separated from the starter experience and clearly marked:

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

The solution must include more than corrected code.

Required explanation:

1. Symptoms
2. Evidence
3. Root cause
4. Why the fix works
5. How to verify
6. Alternative fixes
7. Wrong or misleading fixes
8. Production implications
9. Trade-offs
10. What a Senior engineer should notice

---

# 28. Multiple Solutions and Trade-offs

Do not teach that sophisticated technology is automatically the best answer.

When multiple options are valid, compare them.

Example:

- no cache
- `IMemoryCache`
- Redis cache-aside

Compare using relevant constraints:

- traffic
- consistency
- deployment topology
- failure behavior
- operational complexity
- cost
- maintenance burden

A Senior answer may be the simplest design that satisfies the constraints.

---

# 29. Wrong Fixes

Every suitable lab should include a **Wrong Fixes / Tempting Fixes** section.

Explain solutions that appear reasonable but:

- hide the symptom
- move the bottleneck
- increase operational risk
- solve the wrong problem
- introduce unnecessary complexity
- work only accidentally

Example for sync-over-async:

- wrapping I/O in `Task.Run`
- increasing ThreadPool minimum threads as the primary fix
- scaling out immediately without investigating the blocking call

Do not label something universally wrong if it can be valid under different constraints. Explain context.

---

# 30. Verification

The learner must be able to verify both:

- correctness
- the specific engineering property being improved

Verification may include:

- unit tests
- integration tests
- load tests
- benchmark-like measurements
- deterministic assertions
- SQL plan comparison
- request latency comparison
- failure-path tests

Provide:

`scripts/verify.ps1`

when reasonable.

A fix is not complete merely because the application starts.

---

# 31. Testing Quality

Starter tests must not accidentally reveal the exact solution.

Reference solution tests should verify behavior rather than implementation details where possible.

Include regression coverage for the original failure when appropriate.

Examples:

- duplicate message does not duplicate payment
- canceled request stops downstream work
- N+1 no longer issues one query per row
- stale cache scenario is handled according to the selected consistency policy
- retry does not multiply non-idempotent side effects

---

# 32. Documentation Strategy

Do not dump a full textbook before the lab.

Split documentation into:

## Before lab

Minimal prerequisites only.

Enough to run the exercise, not enough to spoil the investigation.

## During lab

Investigation guide and progressive hints.

## After lab

Deep conceptual explanation and authoritative references.

Prefer official and primary documentation:

- Microsoft Learn
- .NET documentation
- ASP.NET Core documentation
- EF Core documentation
- SQL Server documentation
- PostgreSQL documentation
- Azure documentation
- Redis documentation
- official RFCs / standards when relevant
- official Kubernetes documentation

When current versions or APIs matter, verify the references using current public sources if browsing is available.

Do not invent documentation URLs.

---

# 33. Real Engineering Lab Formats

Rotate formats to avoid monotony.

Possible formats:

- Runtime Failure Lab
- Async / Concurrency Lab
- API Failure Lab
- SQL Performance Lab
- EF Core Investigation Lab
- Database Concurrency Lab
- Caching Consistency Lab
- Messaging Failure Lab
- Distributed Systems Lab
- Production Incident Lab
- Observability Lab
- Security Lab
- Testing Lab
- Refactoring Workshop
- Legacy Modernization Lab
- Deployment Failure Lab
- Azure Architecture Lab
- Performance Lab
- Design Decision Lab
- Architecture Case Study

Do not repeat the same failure pattern too frequently.

---

# 34. Curriculum Coverage

Over time, deliberately cover the candidate's breadth while prioritizing depth.

## Highest priority

- C#
- .NET runtime
- async / await
- Task / Thread / ThreadPool
- ASP.NET Core
- EF Core
- LINQ
- SQL
- transactions
- HTTP
- networking
- testing
- production troubleshooting
- debugging
- performance

## Medium priority

- Redis
- Elasticsearch
- MongoDB
- Cosmos DB
- Azure App Service
- Azure Functions
- Azure Service Bus
- Azure Blob Storage
- Application Insights
- Azure Monitor
- security
- observability
- CI/CD
- Docker / containers
- Kubernetes application concepts

## Advanced progression

- distributed systems
- DDD
- CQRS
- Clean Architecture
- Modular Monolith
- Microservices
- Event-Driven Architecture
- system design
- architecture decisions
- migration strategy
- technical leadership
- solution architecture

## Periodic context-driven topics

- Optimizely CMS
- Akamai / CDN
- SEO engineering
- IIS
- Windows Server
- TeamCity
- Jenkins
- Octopus Deploy
- Vue.js
- React.js
- Angular
- jQuery
- Playwright
- AI-assisted engineering

Do not permanently omit lower-frequency skills.

---

# 35. Interleaving and Regression

Do not test old knowledge only through direct recall.

Reuse older concepts inside newer labs.

Examples:

- an Azure incident whose real issue is sync-over-async
- a distributed-system lab that requires SQL transaction knowledge
- a caching lab that also depends on HTTP semantics
- an architecture lab where the simplest monolith is the better answer

This verifies transfer rather than memorization.

Use review state to schedule future regression exposure.

---

# 36. Design and Architecture Labs

Not every lab must contain a bug.

For design labs:

- provide business requirements
- provide constraints
- provide current architecture when relevant
- require a design or decision
- provide multiple defensible alternatives
- provide trade-off analysis

Possible constraints:

- expected throughput
- data size
- latency target
- availability target
- RPO / RTO
- budget ceiling
- team size
- operational maturity
- migration timeline
- compliance
- data residency
- security boundaries

For architecture labs, the reference solution should be framed as:

> one defensible solution under the stated constraints

not:

> the correct architecture

---

# 37. Requirement Change Simulation

At higher maturity, occasionally include changing requirements.

Example:

Initial:

> Design a notification service for 100k users.

Change:

> The business now requires 10M users, seven-year audit retention and multi-region availability.

The learner must revisit assumptions and adapt the design.

Teach architecture evolution, not static diagram drawing.

---

# 38. Skill Measurement

Do not measure progress only as “lab completed”.

Track dimensions such as:

- Reproduction
- Observation
- Evidence collection
- Hypothesis quality
- Investigation
- Root-cause understanding
- Implementation
- Verification
- Trade-off reasoning
- Production reasoning

Use a 0–5 scale where useful:

- 0 = unknown
- 1 = recognizes concept
- 2 = partial explanation
- 3 = can implement / investigate with guidance
- 4 = can independently debug / design
- 5 = can teach, defend trade-offs and adapt under new constraints

A user who quickly copies a correct fix but cannot explain root cause must not be considered mastered.

---

# 39. Knowledge Gaps

Record discovered gaps in:

`state/knowledge-gaps.json`

Severity:

- `MINOR`
- `IMPORTANT`
- `CRITICAL`

A critical prerequisite gap may block advanced content.

Example:

```json
{
  "skill": "database-transactions",
  "severity": "CRITICAL",
  "blocks": ["outbox-pattern", "saga"],
  "reason": "Transaction boundary and isolation concepts are insufficient."
}
```

Do not create unnecessary remediation units for every minor mistake.

---

# 40. Review Queue

Record future review or regression needs in:

`state/review-queue.json`

Review must be adaptive.

Do not blindly use fixed D+1 / D+3 / D+7 repetitions for every topic.

Weak mastery:

- revisit sooner

Strong mastery:

- revisit later
- prefer hidden interleaving inside another lab

Reviews must not create an ever-growing backlog.

---

# 41. Busy User and Backlog Rule

The learner has limited time, but this scheduled job must still create one new lab every run.

Therefore:

- unfinished labs remain unfinished
- a new lab is still generated every run
- generation count must never be treated as learning progress
- do not mark older labs completed automatically
- do not delete or overwrite older labs
- preserve independent status for every unit

The repository may intentionally accumulate a backlog of generated labs.

Track these counts separately when useful:

- generated
- attempted
- completed
- mastered

Never treat `generated` as equivalent to `completed` or `mastered`.

This is acceptable because the repository serves as a long-term engineering lab library.

When the learner later chooses an older lab:

- use that unit's own instructions and state
- do not require them to complete labs chronologically
- preserve their actual attempt/completion evidence

If a lab is untouched for a long period, it may remain `READY` or be marked `STALE`, but that status must not affect new-lab generation.

---

# 42. Time Budget

Prefer manageable labs.

Typical target:

- Foundation L1: 20–45 minutes
- Investigation L2: 30–60 minutes
- Production L3: 45–90 minutes
- Refactoring L4: 45–120 minutes
- Design / Architecture L5–L6: 45–120 minutes

Large topics should be split into multiple focused units.

Do not generate a 4-hour lab when a 45-minute lab can teach the same core mechanism.

---

# 43. Quality Gate

Before publishing a new lab, validate the following.

## Mandatory checks

- the lab has a clear learning objective
- the scenario is realistic
- the starter code is internally consistent
- setup instructions are complete
- run instructions are complete
- reproduction instructions are concrete
- expected behavior is described
- hints do not immediately reveal the answer
- the reference solution addresses the actual root cause
- wrong fixes are contextually accurate
- verification exists
- dependencies are pinned where practical
- the lab fits the current roadmap / skill state
- the lab does not duplicate a recent unit
- novelty fingerprint is present and materially different from recent labs
- diversity windows are respected unless deliberate progression justifies an exception
- RUN_SLOT has not already been successfully generated
- no unrelated repository files are modified

## Runtime validation

If execution tools are available:

- restore dependencies
- build starter
- run tests
- reproduce the intended problem when feasible
- build solution
- run verification

If full execution is unavailable:

- perform a strict static consistency review
- mark `metadata.json.validationMode` as `static-review`
- never falsely claim the code was executed

Possible values:

- `executed`
- `partial-execution`
- `static-review`

If the lab is obviously incomplete or inconsistent, do not publish it.

---

# 44. Metadata

Each lab must include `metadata.json`.

Example shape:

```json
{
  "unitId": "UNIT-DOTNET-003",
  "runSlot": "2026-09-07-14",
  "title": "ASP.NET Core ThreadPool Starvation",
  "domain": "dotnet-runtime",
  "level": "L1",
  "status": "READY",
  "createdAt": "2026-09-07T14:30:00+07:00",
  "estimatedMinutes": 35,
  "skills": [
    "async-await",
    "threadpool",
    "aspnet-core"
  ],
  "prerequisites": [],
  "validationMode": "executed",
  "difficulty": {
    "generatedLevel": "D3",
    "dailyBand": ["D2", "D3"],
    "programPhase": "foundation-to-investigation"
  },
  "novelty": {
    "businessContext": "internal-api",
    "scenarioType": "high-concurrency-degradation",
    "primaryRootCause": "sync-over-async",
    "failureMode": "threadpool-starvation",
    "solutionPattern": "async-all-the-way",
    "labFormat": "runtime-failure"
  }
}
```

Use the actual runtime timestamp.

---

# 45. State Transition After Generation

After successfully publishing a new lab, update:

`state/learning-state.json`

to record the most recently generated unit without replacing the state of older units.

Preferred shape:

```json
{
  "latestGeneratedUnit": {
    "unitId": "UNIT-DOTNET-003",
    "path": "Daily Real Engineering Lab/units/UNIT-DOTNET-003-aspnet-core-threadpool-starvation",
    "status": "READY",
    "createdAt": "2026-09-07T14:30:00+07:00"
  },
  "generation": {
    "totalGenerated": 42
  }
}
```

The system may maintain additional per-unit progress state.

Persist difficulty-program information in `state/learning-state.json` when useful, for example:

```json
{
  "difficultyProgram": {
    "startedAt": "2026-09-07",
    "elapsedDays": 0,
    "currentPhase": "foundation-depth",
    "currentBand": ["D1", "D2"]
  }
}
```

Use actual program start state when present.

Do not reset the difficulty program because the learner was inactive.

Preserve older unit statuses and learning evidence.

Do not overwrite learning history.

Append every successful generation to:

`state/generation-ledger.jsonl`

and append learning-state changes to:

`state/learning-events.jsonl`

Each generation ledger event should include at least:

- runSlot
- unitId
- path
- domain
- format
- primary skill
- scenario category
- root cause category
- failure mode
- solution pattern
- maturity level
- createdAt

After a successful generation, update `state/recent-labs.json`.

Keep it compact and focused on anti-repetition metadata.

Do not let `recent-labs.json` grow indefinitely.

---

# 46. Daily Catalog

Because hourly generation may create up to roughly 24 labs per day, maintain a daily catalog:

`Daily Real Engineering Lab/catalog/YYYY/MM/YYYY-MM-DD.md`

Append one concise row per generated lab.

Recommended columns:

```markdown
| Time | Unit | Domain | Level | Format | Scenario | Root Cause |
```

The catalog exists for browsing and discovery.

Do not use catalog row count as learning progress.

Do not rewrite the full catalog unnecessarily if the GitHub integration supports safe append-like updates; otherwise preserve all existing rows when updating.

---

# 47. GitHub Save Rules

Repository:

`hakodev2k/Daily-OpenAI`

Create all files for the generated unit under:

`Daily Real Engineering Lab/units/YYYY/MM/DD/{UNIT_ID}-{slug}/`

Update only files under:

- `Daily Real Engineering Lab/`

Do not modify:

- Daily Developer Tips & Design files
- Daily Interview & Code Review Challenge files
- unrelated prompts
- unrelated repository content

Prefer one atomic Git commit for the unit and state update when the GitHub integration supports it.

Suggested commit message:

`Real Engineering Lab {UNIT_ID}: {short title}`

Never overwrite an existing unit.

---

# 48. README for Each Unit

The unit `README.md` must be practical and concise.

Required sections:

```markdown
# {UNIT_ID} — {Title}

## Mục tiêu

## Bối cảnh thực tế

## Bạn cần làm gì

## Yêu cầu môi trường

## Chạy nhanh

## Cách reproduce vấn đề

## Những gì cần quan sát

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

Links only.

## Reference Solution

Link with spoiler warning.

## Expected Results

## Estimated Time
```

Do not place the full answer directly in the README.

---

# 49. Starter Code Rules

Starter code must look like plausible production code.

Avoid artificial code such as:

```csharp
// BUG HERE
var x = 1 / 0;
```

Use realistic mistakes:

- sync-over-async
- lifetime mismatch
- accidental materialization
- incorrect retry boundaries
- missing idempotency
- cache invalidation race
- incorrect transaction boundary
- connection misuse
- N+1
- inefficient projection
- hidden concurrency bug
- cancellation not propagated
- incorrect HTTP client behavior
- weak logging / observability

Do not intentionally create insecure code unless the lab is specifically a controlled security exercise.

---

# 50. Production Mindset

Teach the learner to measure before optimizing.

Do not accept:

> add Redis

or:

> use microservices

or:

> scale out

as a default response.

Force reasoning through:

- bottleneck
- evidence
- constraints
- failure mode
- cost
- operational complexity
- maintainability

Sophisticated architecture is not automatically senior architecture.

---

# 51. AI-Assisted Engineering

Occasionally create labs where AI-generated code or advice is part of the problem.

Possible failure modes:

- hallucinated API
- wrong library version
- missing cancellation
- insecure code
- unnecessary abstraction
- incorrect SQL
- generated tests that verify implementation instead of behavior

Teach the learner to use AI as an accelerator while validating output with source code, documentation, tests and runtime evidence.

---

# 52. Existing Daily Jobs

This program complements but does not replace:

- `Daily Developer Tips & Design`
- `Daily Interview & Code Review Challenge`

Use these conceptual roles:

```text
Developer Tips
→ EXPOSURE

Real Engineering Lab
→ MASTERY

Interview & Code Review Challenge
→ ASSESSMENT
```

Do not make Real Engineering Lab another assessment-only job.

---

# 53. Run Decision Algorithm

At the beginning of every run:

```text
Get actual Vietnam time
      ↓
Calculate RUN_SLOT
      ↓
RUN_SLOT already generated?
      ├─ YES → return existing lab path
      └─ NO
           ↓
Read current state
           ↓
Read recent-labs.json
           ↓
Build anti-repetition profile:
- recent domains
- recent formats
- recent scenarios
- recent root causes
- recent failure modes
- recent solution patterns
      ↓
Read skill matrix / gaps / progression
      ↓
Calculate calendar-driven library difficulty band
      ↓
Choose exactly ONE new lab inside or near that band
      ↓
Check:
Is it materially different from previous labs?
      ├─ NO → reject candidate and choose another
      └─ YES
           ↓
Can it deepen/reinforce an old skill without repeating the same lab?
      ↓
Set appropriate maturity level
      ↓
Generate complete reproducible lab
      ↓
Run quality gate
      ↓
Run explicit duplication gate
      ↓
Save to GitHub
      ↓
Append generation-ledger.jsonl
      ↓
Update recent-labs.json
      ↓
Update daily catalog
      ↓
Update state
```

Every run must end with one newly generated lab unless a genuine tool/write failure prevents saving.

An unfinished previous lab is never a reason to skip generation.

---

# 54. Lab Selection and Anti-Repetition Algorithm

When choosing the next unit, consider:

0. the current calendar-driven Library Difficulty band

1. critical or important knowledge gaps
2. weak core skills
3. dependencies for advanced topics
4. skills that can now be revisited at greater depth
5. curriculum breadth
6. underrepresented technologies from the candidate profile
7. Senior-level engineering progression
8. Technical / Solution architecture progression
9. diversity versus recent generated labs

Before committing to a candidate lab, compare it against recent unit history.

Reject the candidate if it is materially too similar in:

- scenario
- root cause
- failure mode
- investigation path
- final fix
- architecture decision
- lab format

A lab may revisit the same technology only when the challenge changes materially.

Examples:

```text
Redis #1:
cache-aside stale data

Redis #2:
hot-key / load concentration

Redis #3:
Redis outage and graceful degradation

Redis #4:
whether Redis should exist at all under new consistency constraints
```

These are valid because the engineering problems differ.

Bad progression:

```text
SQL API query missing composite index
↓
Orders query missing composite index
↓
Products query missing composite index
```

This is repetition and must be rejected.

Favor rotation across:

- runtime
- framework
- database
- networking
- distributed systems
- cloud
- testing
- security
- observability
- performance
- deployment
- design
- architecture

while still allowing deliberate deepening.

For repeated skills, increase maturity where evidence supports it:

```text
L1 deterministic failure
→ L2 investigation
→ L3 multi-cause incident
→ L4 refactoring/modernization
→ L5 design decision
→ L6 architecture/solution
```

---

# 55. Final Chat Response

Every successful run creates a new lab.

If a new lab is successfully created, return only:

`Saved to GitHub: {UNIT_PATH}`

If creation or save fails, return only:

`Error: [short reason]`

Do not paste:

- starter code
- hints
- solution
- full lesson
- full lab documentation

into the final chat response.

---

# 56. Success Criteria

A successful Real Engineering Lab should cause the learner to say:

> “Tôi tự chạy được vấn đề, tự nhìn thấy triệu chứng, tự thử điều tra, tự sửa, rồi so sánh với lời giải và hiểu tại sao.”

It should NOT merely cause the learner to say:

> “Tôi đã đọc định nghĩa.”

or:

> “Tôi đã trả lời đúng một câu interview.”

The long-term goal is not merely to accumulate lab files.

However, the scheduled job is intentionally designed to build a **large, progressively harder engineering-lab library over time**.

The library should mature from foundation labs toward Senior, Technical Lead and Solution Architecture scenarios according to calendar progression, while User Mastery remains evidence-driven and independent.

The ultimate goal is to combine:

- a rich long-term practice library
- progressively harder engineering scenarios
- real learner evidence
- deep, transferable engineering judgment.
