Generate today’s **Daily Interview & Code Review Challenge** for the current run.

Do not create, modify, explain, or suggest an automation or schedule.

Do not review, summarize, rewrite, or discuss this prompt.

Do not ask follow-up questions.

Return only today’s completed challenge in Vietnamese while preserving technical terms, APIs, code, commands, framework names, Azure service names, architectural terminology, database terminology and product names in their original form.

Use the actual current Vietnam time:

`Asia/Ho_Chi_Minh / UTC+7`

---

# 1. Objective

Create a recurring practical assessment for a .NET-focused engineer progressing through:

**Middle Developer → Senior .NET Developer → Technical Lead → Software / Solution Architect**

The challenge must develop and evaluate:

* programming depth
* runtime understanding
* debugging ability
* production troubleshooting
* database reasoning
* API engineering
* cloud engineering
* distributed-system reasoning
* software architecture
* performance
* security
* observability
* testing
* DevOps and deployment
* frontend/backend integration
* CMS and e-commerce engineering
* technical decision-making
* code-review ability
* ability to explain engineering trade-offs

Do not optimize the content for memorizing interview answers.

Optimize it for becoming better at:

* real engineering work
* technical interviews
* code review
* system design
* production incidents
* architecture discussions

Difficulty must come from realistic ambiguity, runtime behavior, failure modes, trade-offs and incomplete information.

Avoid obscure trivia.

---

# 2. Daily Mode

Determine the current calendar day using the actual Vietnam date.

## EVEN day-of-month

Generate exactly:

**3 Interview Questions**

## ODD day-of-month

Generate exactly:

**2 Engineering Code Review Challenges**

Examples only:

* day 4 → Interview Questions
* day 5 → Code Review Challenges

Do not derive the date from examples, schedule metadata or previous runs.

State the selected mode briefly at the beginning of the generated Markdown.

---

# 3. Candidate Profile

Personalize the challenges for the following engineering profile.

## Primary engineering stack

* C#
* .NET
* .NET 6
* .NET 8
* .NET 10
* ASP.NET Core
* ASP.NET MVC
* ASP.NET Web API
* Entity Framework Core
* LINQ
* SQL
* SQL Server
* PostgreSQL
* RESTful APIs

## Legacy / modernization experience

* .NET Framework 4.5
* .NET Core 3.1
* .NET migration
* legacy modernization
* backward compatibility
* refactoring
* production migration

## NoSQL, cache and search

* MongoDB
* Cosmos DB
* Redis
* Elasticsearch
* Kibana
* caching
* query optimization
* index optimization
* full-text search

## Microsoft Azure

* Azure App Service
* Azure Functions
* Azure Service Bus
* Azure Blob Storage
* Azure Monitor
* Application Insights
* Azure AD / Microsoft Entra ID concepts
* Azure AD B2C
* Managed Identity
* Azure Key Vault

## Architecture & backend patterns

* Clean Architecture
* Onion Architecture
* Domain-Driven Design
* CQRS
* MediatR
* Repository Pattern
* Unit of Work
* SOLID
* Design Patterns
* dependency injection
* asynchronous programming
* background services
* hosted services
* scheduled jobs
* third-party API integrations

## Frontend / full-stack

* JavaScript
* TypeScript
* Vue.js
* React.js
* Angular
* jQuery
* HTML5
* CSS3

Prioritize Vue.js and React.js for modern frontend questions.

Use Angular and jQuery primarily in legacy-system, maintenance or modernization scenarios.

## CMS, e-commerce and content systems

* Optimizely CMS
* CMS architecture
* page types
* content blocks
* content modeling
* composable commerce
* e-commerce integrations
* marketing websites
* CRM-related systems
* third-party product/authentication/review integrations

## DevOps & hosting

* Git
* Azure Repos
* CI/CD
* TeamCity
* Jenkins
* Octopus Deploy
* IIS
* Windows Server
* Azure App Service on Linux
* Kubernetes

## Testing & quality

* Playwright
* unit testing concepts
* integration testing
* end-to-end testing
* SonarQube
* static analysis
* code review
* refactoring

## Observability & production

* Azure Monitor
* Application Insights
* Kibana
* Elasticsearch
* structured logging concepts
* production monitoring
* production troubleshooting
* hotfixes
* incident investigation

## Performance & web engineering

* SQL optimization
* query/index optimization
* Redis caching
* Akamai cache/CDN
* Response Compression
* API performance
* website performance
* Google Lighthouse concepts
* SEO engineering
* accessibility and loading performance

## Engineering process

* SDLC
* Agile
* Scrum
* Sprint Planning
* estimation
* requirement analysis
* technical solution design
* code review
* collaboration with QA
* collaboration with Product Owner / Product Manager
* production support

## AI-assisted engineering

The candidate is proficient in using AI tools for software engineering.

Challenges may include:

* AI-assisted repository analysis
* AI-assisted debugging
* AI-assisted code review
* test generation
* migration assistance
* documentation
* agent workflows
* prompt/context design
* verification of AI-generated code

Do not invent commercial project experience with a specific AI platform unless supported by available history.

---

# 4. Experience Grounding Rule

Never invent candidate experience.

Technologies in the curriculum may be used in one of three ways:

### Experienced

Technology clearly represented in the candidate profile.

Ask deeper production and architecture questions.

### Adjacent

Technology closely related to existing skills.

It may be introduced as a design alternative.

### New

Technology not supported by the candidate profile.

Do not write the scenario as though the candidate has already used it professionally.

If such a technology is useful, frame it explicitly as:

* an alternative to evaluate
* a new technology being considered
* an unfamiliar dependency requiring reasoning from fundamentals

---

# 5. Curriculum Coverage

The challenge system must eventually cover the entire candidate profile.

Do not attempt to test every technology in every run.

Instead maintain broad rotation across the following curriculum groups.

---

## Group A — C# Language & Runtime

Topics may include:

* value types vs reference types
* records
* structs
* generics
* collections
* delegates
* events
* nullable reference types
* pattern matching
* exception handling
* IDisposable
* IAsyncDisposable
* memory allocation
* Garbage Collector
* LOH
* boxing
* strings
* Span<T>
* Memory<T>
* reflection
* serialization
* immutability
* dependency injection
* configuration
* Options Pattern

Prefer practical runtime behavior over language trivia.

---

## Group B — Async, Concurrency & Background Processing

Include:

* async / await
* Task
* ValueTask
* CancellationToken
* ThreadPool
* I/O-bound vs CPU-bound
* synchronization
* race conditions
* thread safety
* SemaphoreSlim
* Task.WhenAll
* Parallel.ForEachAsync
* BackgroundService
* HostedService
* scheduled jobs
* graceful shutdown
* duplicate execution
* multiple application instances

Important production scenario:

> A scheduled/background operation starts executing multiple times after horizontal scaling.

---

## Group C — ASP.NET Core & HTTP

Include:

* request pipeline
* middleware
* routing
* model binding
* filters
* validation
* dependency injection lifetimes
* error handling
* ProblemDetails
* REST API design
* HTTP semantics
* API Versioning
* pagination
* filtering
* response compression
* file streaming
* timeout
* cancellation
* health checks
* Swagger
* OpenAPI
* third-party integrations

---

## Group D — Authentication & Security

Include:

* Authentication vs Authorization
* JWT
* claims
* policy-based authorization
* Azure AD / Microsoft Entra ID
* Azure AD B2C
* Managed Identity
* Key Vault
* API security
* CORS
* CSRF
* XSS
* SQL injection
* SSRF
* secrets
* sensitive logging
* file upload security
* authorization mistakes
* least privilege
* service-to-service identity

---

## Group E — Entity Framework Core

Include:

* DbContext lifetime
* change tracking
* AsNoTracking
* IQueryable
* deferred execution
* LINQ translation
* Include
* projections
* N+1
* transactions
* concurrency
* migrations
* SaveChanges
* query performance
* connection resiliency
* database round trips

Challenge assumptions around:

* Repository Pattern
* Unit of Work
* direct DbContext usage

Do not imply one approach is always correct.

---

## Group F — SQL Server & PostgreSQL

Include:

* indexes
* composite indexes
* execution plans
* statistics
* joins
* pagination
* keyset pagination
* transactions
* isolation levels
* locking
* blocking
* deadlocks
* data types
* schema design
* normalization
* denormalization
* constraints
* SQL injection
* query optimization
* connection pooling

Questions should often require diagnosing why something is slow rather than simply recommending an index.

---

## Group G — MongoDB & Cosmos DB

Include:

* document modeling
* denormalization
* indexes
* query patterns
* partitioning
* partition keys
* hot partitions
* Cosmos DB RU concepts
* consistency models
* query cost
* transaction boundaries
* SQL vs NoSQL decisions

---

## Group H — Redis & Caching

Include:

* cache-aside
* distributed cache
* TTL
* invalidation
* cache stampede
* stale data
* serialization
* hot keys
* cache key design
* session cache
* distributed locking
* cache outage
* fallback behavior
* consistency

Never reduce the answer to:

“Use Redis.”

Ask what happens when Redis fails.

---

## Group I — Elasticsearch & Search

Include:

* inverted index
* text vs keyword
* analyzers
* tokenization
* filtering
* relevance
* autocomplete
* geo search
* indexing
* reindexing
* search performance
* synchronization with relational databases
* eventual consistency
* search failures

---

## Group J — Azure App Service

Include:

* deployment
* Linux hosting
* configuration
* scaling
* multiple instances
* deployment slots
* health checks
* application restart behavior
* environment settings
* logs
* networking concepts
* Managed Identity
* production troubleshooting

---

## Group K — Azure Functions

Include:

* triggers
* bindings
* retries
* cold starts
* idempotency
* poison work
* scheduled execution
* asynchronous processing
* execution lifetime
* observability
* Functions vs Worker Service

---

## Group L — Azure Service Bus

Include:

* queues
* topics
* subscriptions
* competing consumers
* retries
* duplicate delivery
* at-least-once delivery
* dead-letter queue
* message lock
* ordering
* sessions
* idempotent consumers
* asynchronous workflows

---

## Group M — Azure Blob Storage

Include:

* file upload
* file streaming
* SAS
* Managed Identity
* permissions
* metadata
* lifecycle
* large files
* backend proxy vs direct upload
* failure recovery

---

## Group N — Architecture

Include:

* Clean Architecture
* Onion Architecture
* Layered Architecture
* DDD
* CQRS
* MediatR
* Repository
* Unit of Work
* SOLID
* design patterns
* Modular Monolith
* Microservices
* Event-Driven Architecture
* service boundaries

Do not teach architecture as folder structure.

Require discussion of:

* dependencies
* boundaries
* ownership
* transaction boundaries
* operational impact
* maintainability
* complexity

---

## Group O — Distributed Systems

Include:

* synchronous vs asynchronous communication
* partial failure
* retry
* timeout
* exponential backoff
* idempotency
* eventual consistency
* duplicate delivery
* Outbox Pattern
* Inbox Pattern
* Saga concepts
* distributed transactions
* service failure
* dependency degradation

---

## Group P — Observability & Production Troubleshooting

Include:

* Azure Monitor
* Application Insights
* Kibana
* Elasticsearch logs
* structured logging
* correlation ID
* metrics
* tracing
* request telemetry
* dependency telemetry
* alerting
* production incident investigation
* root-cause analysis

Use:

**symptom → evidence → hypothesis → verification → fix**

---

## Group Q — Performance

Include:

* API latency
* CPU
* memory
* GC
* ThreadPool starvation
* SQL performance
* cache
* network
* serialization
* response compression
* CDN
* connection pools
* load
* p50 / p95 / p99
* profiling
* bottleneck analysis

Prefer:

**measure before optimize**

---

## Group R — Vue.js / React.js / JavaScript / TypeScript

Include:

* component design
* state
* reactivity
* hooks
* lifecycle
* TypeScript
* promises
* async
* event loop
* API integration
* authentication
* frontend error handling
* browser caching
* rendering
* performance
* XSS
* CORS

Most frontend challenges should test a backend/full-stack engineer rather than a dedicated frontend specialist.

---

## Group S — Legacy Frontend

Use occasionally:

* Angular
* jQuery
* HTML5
* CSS3

Preferred scenarios:

* legacy maintenance
* gradual modernization
* integration with ASP.NET Core
* performance
* technical debt

---

## Group T — Optimizely CMS & CMS Architecture

Include:

* page types
* blocks
* content types
* content modeling
* editorial workflow
* CMS caching
* content delivery
* search
* APIs
* upgrade risks
* performance
* headless vs traditional CMS

Use Optimizely as an implementation example of broader CMS engineering.

---

## Group U — E-commerce & Composable Systems

Include:

* CMS
* product information
* authentication providers
* review providers
* search
* CDN
* cache
* external APIs
* failure isolation
* dependency degradation
* composable commerce
* ownership boundaries

---

## Group V — Akamai, CDN & Web Performance

Include:

* CDN
* edge caching
* browser caching
* Cache-Control
* TTL
* purge
* stale content
* origin
* Response Compression
* gzip / Brotli concepts
* static vs dynamic content
* Core Web Vitals
* Lighthouse

---

## Group W — SEO Engineering

Include engineering topics such as:

* canonical URLs
* redirects
* status codes
* sitemap
* robots.txt
* metadata
* structured data
* crawlability
* performance
* Lighthouse
* accessibility
* dynamic content

Do not ask marketing questions.

---

## Group X — Testing & Quality

Include:

* unit testing
* integration testing
* API testing
* Playwright
* end-to-end testing
* mocking
* flaky tests
* test data
* concurrency tests
* failure-path tests
* SonarQube
* static analysis
* code review
* refactoring

---

## Group Y — Git & CI/CD

Include:

* Git
* Azure Repos
* branching
* merge
* rebase
* cherry-pick
* Git bisect
* pull requests
* CI
* CD
* release pipelines

Tools may include:

* TeamCity
* Jenkins
* Octopus Deploy

Focus on principles rather than memorizing UI steps.

---

## Group Z — Hosting & Infrastructure

Include:

* IIS
* Windows Server
* Linux hosting
* App Pools
* process recycling
* SSL
* configuration
* environment variables
* application startup
* deployment failures
* filesystem permissions
* logs
* reverse proxy concepts

---

## Group AA — Kubernetes

Include:

* Pods
* Deployments
* Services
* ConfigMaps
* Secrets
* readiness
* liveness
* rolling deployments
* requests / limits
* pod restarts
* container troubleshooting
* application configuration

Keep the scope appropriate for an application developer.

---

## Group AB — .NET Modernization

Include scenarios such as:

`.NET Framework 4.5`
→ `.NET Core 3.1`
→ `.NET 6`
→ `.NET 8`
→ modern .NET

Test:

* dependency compatibility
* migration strategy
* breaking changes
* hosting changes
* configuration
* APIs
* testing
* rollout
* rollback
* incremental modernization
* technical debt

Do not assume rewrite-from-scratch is the best solution.

---

## Group AC — Engineering Process

Include:

* requirement clarification
* technical analysis
* sprint estimation
* technical design
* collaboration with QA
* collaboration with Product
* release planning
* production support
* incident communication
* code review
* technical debt
* prioritization

---

## Group AD — AI-Assisted Development

Occasionally test how AI should be used safely in engineering.

Include:

* code generation
* refactoring
* debugging
* test generation
* repository exploration
* architecture analysis
* migration
* documentation
* code review

Evaluate:

* verification
* source grounding
* tests
* false assumptions
* hallucinated APIs
* insecure output
* unnecessary abstractions

Do not require knowledge of a specific AI vendor unless supported by current context.

---

# 6. Long-Term Skill Coverage Rule

Use recent saved challenges when accessible to maintain a **skill coverage matrix**.

Do not display the matrix.

Track curriculum groups conceptually as:

* not recently tested
* recently tested
* repeated
* needs deeper revisit

Across approximately **14–21 runs**, make a deliberate effort to cover all major skills represented in the candidate profile.

Do not allow high-frequency topics such as:

* async/await
* EF Core
* Redis
* microservices

to crowd out less frequently tested skills such as:

* Optimizely CMS
* Elasticsearch
* Akamai
* IIS
* Windows Server
* TeamCity
* Octopus Deploy
* Vue.js
* SEO
* Playwright
* Cosmos DB
* Azure Service Bus
* Kubernetes

Lower-frequency skills do not need equal frequency, but they must not disappear from the curriculum.

---

# 7. Difficulty Progression

Progress recurring skills through:

**implementation**
→ **internal mechanism**
→ **debugging**
→ **production failure**
→ **performance**
→ **trade-offs**
→ **architecture decision**

Example:

### EF Core

Early:
tracking vs no-tracking

Later:
LINQ translation

Later:
N+1

Later:
transaction/concurrency

Later:
production SQL investigation

Later:
Repository vs DbContext design decision

### Redis

Early:
cache-aside

Later:
TTL

Later:
invalidation

Later:
stampede

Later:
Redis outage

Later:
whether cache is justified

Repeated topics must deepen rather than duplicate previous questions.

---

# 8. General Quality Rules

Every challenge must test practical engineering judgment.

Prefer scenarios requiring the candidate to:

* clarify incomplete requirements
* state assumptions
* explain internal behavior
* investigate production symptoms
* form multiple hypotheses
* collect evidence
* prioritize risk
* compare alternatives
* explain trade-offs
* consider failure
* consider security
* consider performance
* consider maintainability
* explain testing
* explain monitoring
* communicate clearly

Avoid simple definition questions such as:

* What is async/await?
* What is Redis?
* What is CQRS?
* What is an index?
* What is Azure Service Bus?
* What is Kubernetes?

Put the technology into a realistic scenario.

---

# 9. Scenario Grounding

Prefer realistic scenarios derived from systems similar to:

* internal employee platform
* attendance platform
* booking system
* approval workflows
* timesheet system
* enterprise management portal
* large e-commerce platform
* composable commerce
* CMS
* product catalog
* event-management platform
* marketing campaign platform
* automotive marketplace
* CRM
* search platform
* marketing websites
* mobile backend
* background-processing system
* file-processing service

Do not copy proprietary business details.

Use generic but realistic variants.

---

# 10. EVEN DAY — Interview Question Mode

Generate exactly **3 interview questions**.

The three questions must test different engineering dimensions.

---

# Question 1 — Internal Mechanism + Practical Behavior

Choose a topic mainly from:

* C# runtime
* async/concurrency
* ASP.NET Core
* EF Core
* SQL
* HTTP
* DI lifetime
* memory/GC
* Redis
* Cosmos DB
* Elasticsearch
* authentication
* frontend JavaScript runtime when appropriate

Require the candidate to explain:

1. what happens internally
2. why the observed behavior occurs
3. which assumptions are incorrect
4. production impact
5. how the explanation could be verified

Do not accept a definition-only response.

---

# Question 2 — Production Incident

Use a realistic incident involving one or more CV-related technologies.

Possible scenarios:

* API latency increases
* CPU spikes
* memory grows
* SQL becomes slow
* EF Core causes excessive queries
* connection pool exhaustion
* Redis returns stale data
* Redis becomes unavailable
* Elasticsearch search results lag
* Cosmos DB request cost spikes
* Service Bus backlog grows
* Service Bus messages are duplicated
* Azure Function retries
* BackgroundService processes data twice
* App Service begins returning 5xx
* authentication stops working
* Akamai serves stale content
* CMS pages become slow
* Kubernetes pod keeps restarting
* deployment introduces regression
* external API times out
* frontend makes duplicate API calls
* IIS application pool repeatedly recycles

Require the candidate to structure the response around:

1. user/business impact
2. immediate stabilization
3. evidence collection
4. competing hypotheses
5. investigation
6. root cause
7. short-term remediation
8. long-term prevention
9. testing
10. monitoring

Do not imply that one root cause is predetermined.

---

# Question 3 — Architecture / System Design / Engineering Decision

Rotate among:

### Architecture / system design

Possible systems:

* internal employee platform
* document/file service
* event-management system
* notification system
* CMS-backed website
* e-commerce integration layer
* search platform
* background-processing platform
* approval workflow
* mobile backend
* automation/testing platform

Require discussion of:

* requirements
* scale assumptions
* APIs
* data model
* boundaries
* storage
* cache
* asynchronous processing
* consistency
* security
* reliability
* observability
* deployment
* failure recovery
* trade-offs

### Engineering decision

Examples:

* Clean Architecture vs simpler layers
* Repository vs direct DbContext
* SQL vs Cosmos DB
* Redis vs no cache
* REST vs Service Bus
* Functions vs BackgroundService
* Modular Monolith vs Microservices
* server-side processing vs CDN caching
* Vue modernization decisions
* CMS integration boundaries
* legacy migration strategy
* Kubernetes vs simpler App Service hosting

The problem must have multiple defensible solutions.

Do not automatically reward complexity.

---

# 11. Interview Question Output Format

For every question use:

# Câu [N] — [Tiêu đề]

**Loại câu hỏi:**
**Độ khó:** Intermediate / Senior / Senior+
**Thời gian trả lời đề xuất:**
**Kỹ năng được đánh giá:**

## Bối cảnh

Describe:

* system
* relevant technologies
* expected behavior
* scale
* symptoms or requirement
* business/user impact
* relevant constraints

Deliberately leave some information unspecified.

The candidate should need clarification.

## Câu hỏi chính

Ask one primary question requiring substantial reasoning.

## Câu hỏi làm rõ nên cân nhắc

Provide **3–5** valuable clarification questions.

Do not merely provide questions whose answers are obvious from the scenario.

## Gợi ý tư duy

### Gợi ý 1

Direction only.

### Gợi ý 2

Important technical areas.

### Gợi ý 3

Stronger technical direction.

Do not reveal the complete answer too early.

## Cấu trúc trả lời đề xuất

Provide a question-specific framework.

Typical example:

1. Clarify
2. State assumptions
3. Assess impact
4. Explain mechanism
5. Gather evidence
6. Compare hypotheses / options
7. Recommend
8. Explain implementation
9. Test
10. Monitor

Do not blindly reuse identical steps if inappropriate.

## Ý chính của câu trả lời tốt

### Bắt buộc

### Điểm cộng

### Senior+

## Hướng giải quyết tham khảo

Give a detailed technically sound reference answer covering where applicable:

* assumptions
* mechanism
* reasoning
* implementation
* alternatives
* trade-offs
* performance
* security
* reliability
* failure behavior
* testing
* monitoring
* deployment
* circumstances that would change the recommendation

## Mẫu trả lời trong phỏng vấn

Provide a concise natural spoken response suitable for approximately **2–4 minutes**.

It should:

* state assumptions
* explain reasoning
* mention trade-offs
* avoid sounding memorized

## Câu hỏi đào sâu

Provide exactly **3** likely follow-ups.

At least one must challenge the candidate's recommendation.

## Lỗi trả lời thường gặp

Provide **3–5** question-specific mistakes.

## Thang điểm 0–10

Evaluate:

* requirement clarification
* technical correctness
* depth
* debugging / reasoning
* trade-off analysis
* production readiness
* communication

---

# 12. Interview Difficulty Rules

Across the 3 interview questions:

* at least **1 must be Senior**
* at least **1 must be Senior or Senior+**
* no more than **1 may be Intermediate**

The three questions must not all center on one technology.

At least two different major curriculum groups must appear.

Prefer three or more.

---

# 13. ODD DAY — Engineering Code Review Mode

Generate exactly **2 independent Code Review Challenges**.

Unlike the previous version, challenges do not always need to be pure backend C# if doing so would prevent coverage of the candidate's actual skills.

However:

* at least **1 challenge must contain C#/.NET code**
* the second may use C#/.NET, SQL, TypeScript/Vue/React, configuration, Docker/Kubernetes YAML, CI/CD configuration, or a mixed full-stack snippet when appropriate

When non-C# code is used, it must still evaluate software-engineering skills relevant to the candidate profile.

---

# 14. Code Review Topic Pool

Rotate across:

## C# / .NET

* async
* CancellationToken
* race conditions
* locking
* DI lifetimes
* resource disposal
* HttpClient usage
* BackgroundService
* error handling
* memory allocation
* logging

## ASP.NET Core

* middleware
* controllers
* APIs
* validation
* authentication
* authorization
* ProblemDetails
* file handling
* response compression

## EF Core

* tracking
* N+1
* IQueryable
* transaction handling
* concurrency
* DbContext lifetime
* query performance

## SQL

* inefficient queries
* indexes
* locking
* correctness
* SQL injection
* pagination

## Redis

* stale data
* invalidation
* stampede
* failure handling
* serialization
* distributed locks

## Azure Service Bus

* duplicate processing
* idempotency
* retries
* DLQ
* message settlement

## Azure Functions / Background Processing

* retries
* duplicate execution
* cancellation
* long-running work
* failure handling

## Cosmos DB

* poor partition key
* expensive query pattern
* consistency assumptions

## Elasticsearch

* incorrect mapping
* expensive query
* primary database synchronization
* eventual consistency

## Security

* missing authorization
* trusting claims incorrectly
* secrets exposure
* sensitive logging
* insecure file upload
* injection
* XSS

## Frontend

* Vue.js
* React.js
* TypeScript
* async race conditions
* duplicate requests
* stale state
* lifecycle misuse
* error handling
* XSS
* API integration

## CMS / Web

* caching
* page/content modeling
* content query inefficiency
* CDN interaction
* stale content
* CMS service boundaries

## DevOps

* broken deployment pipeline
* environment configuration
* secret leakage
* unsafe database migrations
* missing rollback
* deployment-order issues

## Kubernetes

* probes
* resource limits
* environment configuration
* Secrets
* deployment stability

---

# 15. Code Review Realism Rules

Do not use trivial syntax errors.

The code/configuration should appear plausible.

It may compile or deploy successfully while still containing serious problems.

Problems should represent issues such as:

* correctness
* concurrency
* performance
* security
* reliability
* maintainability
* data consistency
* failure handling
* observability

At least one of the two challenges must contain a problem that becomes visible mainly under:

* concurrency
* load
* network failure
* dependency failure
* retry
* deployment
* production scale

---

# 16. Code Review Snippet Size

Normal target:

* C#/TypeScript: approximately **20–70 lines**
* SQL: enough statements to form a realistic review
* YAML/configuration: enough context to expose architectural or deployment problems

Do not artificially pad snippets.

---

# 17. Code Review Output Format

Use:

# Code Review Challenge [N] — [Tiêu đề]

**Chủ đề:**
**Độ khó:** Intermediate / Senior / Senior+
**Thời gian review đề xuất:**
**Số vấn đề cần tìm:** approximate count only

## Bối cảnh

Explain:

* intended behavior
* application/system
* runtime environment
* scale/concurrency
* production symptoms
* constraints

## Đoạn code cần review

Provide the complete code/configuration block.

Do not mark incorrect lines.

Do not include comments revealing the problems.

## Nhiệm vụ của ứng viên

Ask the candidate to:

1. identify correctness problems
2. identify performance/security/reliability concerns
3. explain production impact
4. prioritize problems
5. propose corrected implementation/design
6. explain testing
7. explain production telemetry

## Câu hỏi gợi mở

Provide **3–5** guiding questions without revealing the answer.

## Gợi ý

### Gợi ý 1

Reveal the primary risk category.

### Gợi ý 2

Point toward relevant runtime/database/distributed/security behavior.

Do not provide a third hint.

## Dừng lại và tự review

---

**Hãy tự review đoạn code trước khi xem phần phân tích bên dưới.**

---

## Kết quả code review tham khảo

For each issue provide:

### [Severity] — [Issue]

**Vấn đề:**
**Tại sao xảy ra:**
**Ảnh hưởng production:**
**Cách sửa:**

Allowed severities:

* Critical
* High
* Medium
* Low

Order findings by severity.

## Phiên bản đề xuất

Provide corrected or substantially improved code/configuration.

Fix important engineering problems, not only naming/style.

## Giải thích thiết kế sau khi sửa

Explain key choices.

## Trade-offs

Explain the meaningful costs of the improved solution.

## Cách kiểm thử

Provide **3–5** high-value tests.

Use appropriate types:

* unit test
* integration test
* concurrency test
* load test
* database test
* security test
* failure-path test
* E2E test
* deployment validation

## Quan sát trong production

Recommend relevant:

* logs
* metrics
* traces
* alerts
* database observations
* queue observations
* Azure telemetry
* browser/network observations where relevant

## Câu hỏi phỏng vấn mở rộng

Provide **2–3** follow-up questions.

---

# 18. Code Review Diversity Rules

The two daily challenges must cover different primary categories.

Bad combination:

* EF Core N+1
* another EF Core N+1

Good combinations:

* C# concurrency + SQL performance
* Service Bus idempotency + Vue API race
* ASP.NET Core authorization + Redis caching
* EF Core transaction + Kubernetes configuration
* BackgroundService + Elasticsearch consistency
* CMS caching + SQL query design

---

# 19. Architecture Judgment Rules

Never automatically recommend:

* Microservices
* Kubernetes
* CQRS
* Event Sourcing
* DDD
* Clean Architecture
* distributed cache
* messaging

The candidate should be rewarded for recognizing when a simpler design is sufficient.

Examples:

* layered architecture instead of complex Clean Architecture
* Modular Monolith instead of Microservices
* App Service instead of Kubernetes
* direct DbContext instead of generic repository
* synchronous call instead of messaging for a simple local workflow
* no cache when database performance is already sufficient

Senior-level reasoning must consider **cost of complexity**.

---

# 20. Production Incident Reasoning

For operational scenarios encourage:

**impact**
→ **stabilize**
→ **collect evidence**
→ **form multiple hypotheses**
→ **narrow down**
→ **identify root cause**
→ **remediate**
→ **verify**
→ **prevent recurrence**

Do not jump directly from symptom to solution.

---

# 21. Performance Rules

For performance problems require measurement.

Evidence may include:

* request duration
* p95/p99
* CPU
* memory
* allocations
* GC
* SQL execution plan
* database waits
* query count
* connection pool
* ThreadPool
* cache hit ratio
* Service Bus queue depth
* Cosmos DB RU consumption
* Application Insights dependencies
* network waterfall
* CDN cache status

Avoid:

> “Add Redis.”

unless evidence supports caching.

---

# 22. Observability Rules

Treat observability as part of design.

Use relevant tools from the candidate profile:

* Azure Monitor
* Application Insights
* Kibana
* Elasticsearch

When applicable evaluate:

* log quality
* structured logging
* correlation IDs
* metrics
* tracing
* dashboards
* alerts

A good answer should explain what telemetry would prove or disprove a hypothesis.

---

# 23. Security Rules

Security questions and reviews should be practical.

Potential areas:

* broken authorization
* JWT misuse
* claims trust
* Azure identity
* Managed Identity
* Key Vault
* sensitive logging
* XSS
* SQL injection
* CORS
* file-upload risks
* external API credentials
* dependency secrets
* least privilege

Do not make security purely theoretical.

---

# 24. Modernization Rules

Periodically include scenarios around:

* .NET Framework → modern .NET
* .NET Core 3.1 → .NET 6/8
* legacy Angular/jQuery
* IIS/Windows Server → App Service/Linux/container options

Test the candidate on:

* migration risk
* compatibility
* rollout
* testing
* observability
* rollback
* incremental modernization

Do not assume a full rewrite is ideal.

---

# 25. CMS / E-commerce Rules

Periodically include questions using:

* Optimizely CMS
* content blocks
* page types
* content modeling
* cache
* CDN
* search
* third-party integrations
* composable commerce

Evaluate broader engineering reasoning rather than Optimizely API memorization.

---

# 26. Frontend Rules

Vue.js / React.js / TypeScript challenges should focus on full-stack concerns such as:

* API calls
* authentication
* state correctness
* race conditions
* browser performance
* XSS
* error handling
* request cancellation
* caching
* network behavior

Avoid specialized CSS trivia.

---

# 27. DevOps / Deployment Rules

Questions may use:

* Git
* Azure Repos
* TeamCity
* Jenkins
* Octopus Deploy
* IIS
* Windows Server
* Azure App Service
* Kubernetes

Evaluate:

* release safety
* artifact consistency
* configuration
* secrets
* migration ordering
* rollback
* health checks
* observability

Do not test UI-specific knowledge of CI/CD tools.

---

# 28. Testing Rules

A strong answer should choose tests based on risk.

Possible tests:

* unit
* integration
* API
* database
* Playwright E2E
* concurrency
* load
* security
* failure injection

Do not reward “100% code coverage” as a sufficient testing strategy.

---

# 29. AI-Assisted Engineering Rules

AI-related questions should normally be engineering-decision questions, not vendor trivia.

Evaluate:

* what work AI performs
* what must remain deterministic
* what context is supplied
* how generated output is verified
* how tests validate changes
* how hallucinations are caught
* how repository source is grounded
* permissions
* security
* human review

Do not assume candidate experience with a particular AI product unless supported.

---

# 30. Previous Run Awareness

When recent challenge files in the repository are available:

inspect them before selecting today's challenge topics.

Use history to avoid:

* repeated question scenarios
* repeated bug patterns
* repeated snippets
* repeated reference solutions

Revisit weak or important concepts only using:

* a deeper mechanism
* a different failure mode
* a new context
* a more difficult trade-off

Do not mention repository-history inspection in the generated lesson.

---

# 31. Optional Performance Feedback History

If previous candidate:

* answers
* scores
* missed issues
* strengths
* weaknesses

are available, use them.

Prioritize weak areas without making every challenge about the same weakness.

Never invent previous scores.

---

# 32. Final Summary

At the end include:

# Tổng kết hôm nay

## Kiến thức trọng tâm

Summarize the most important mechanisms or engineering principles from today's challenge.

## Mẫu tư duy cần nhớ

Provide **3–5** reusable engineering reasoning patterns.

Examples:

* Clarify before designing
* Measure before optimizing
* Stabilize before deep investigation
* Design retry together with idempotency
* Observe before guessing
* Simpler architecture is often the safer architecture
* Treat external systems as failure-prone
* Test the failure path
* Validate AI-generated changes

## Nhật ký luyện tập

### Interview mode

* Question 1 score:
* Question 2 score:
* Question 3 score:
* Strongest area:
* Weakest area:
* Concept to revisit:

### Code Review mode

* Challenge 1 issues found:
* Challenge 2 issues found:
* Highest-severity issue missed:
* Strongest review area:
* Weakest review area:
* Concept to revisit:

Include only the section corresponding to today's selected mode.

Do not pre-fill scores.

---

# 33. Accuracy Rules

Use current official documentation when claims depend on current or version-specific behavior.

Especially verify when necessary:

* current .NET
* ASP.NET Core
* EF Core
* Microsoft Azure
* Optimizely CMS
* Kubernetes
* Vue.js
* React.js
* security behavior
* Azure Service Bus
* Azure Functions
* Cosmos DB

Do not fabricate:

* runtime behavior
* limits
* pricing
* service guarantees
* benchmarks
* API availability
* product features

Clearly distinguish:

* scenario assumptions
* confirmed technology behavior
* recommended design choices

---

# 34. Accuracy vs Interview Ambiguity

The scenario may intentionally omit information.

However, the **reference answer must not silently invent the missing information**.

Instead say things like:

* “Nếu traffic ở mức X thì…”
* “Nếu requirement yêu cầu strong consistency thì…”
* “Nếu hệ thống chạy nhiều instance thì…”
* “Cần xác minh… trước khi quyết định…”

Reward conditional reasoning.

---

# 35. EVEN DAY Final Validation

Before saving verify:

1. Exactly 3 Interview Questions exist.
2. Question 1 tests an internal mechanism.
3. Question 2 is a realistic production incident.
4. Question 3 tests architecture/system design/engineering decision.
5. At least two major technology groups are represented.
6. At least one question is Senior.
7. No more than one question is Intermediate.
8. Every question includes:

   * clarification
   * hints
   * answer framework
   * reference answer
   * interview answer
   * follow-ups
   * mistakes
   * scoring
9. No question is answerable well by merely defining a term.

---

# 36. ODD DAY Final Validation

Before saving verify:

1. Exactly 2 Code Review Challenges exist.
2. At least one challenge uses C#/.NET.
3. The challenges cover different primary areas.
4. The code/configuration is plausible.
5. Problems are not trivial syntax errors.
6. At least one challenge contains a load/concurrency/failure issue.
7. Problematic lines are not identified before the answer.
8. Corrected code/design is included.
9. Testing is included.
10. Production observability is included.
11. Important trade-offs are discussed.

---

# 37. Content Restrictions

Do not add:

* motivational content
* generic career advice
* certifications
* course recommendations
* unrelated news
* homework outside today's challenge
* bonus interview questions
* bonus code-review challenges

Interview day:

exactly **3 questions**.

Code Review day:

exactly **2 challenges**.

---

# 38. Runtime Date & Timestamp

At the beginning of the run, obtain the actual current date/time for:

`Asia/Ho_Chi_Minh`

using an available runtime/time tool.

Do not infer the current time.

Do not use:

* automation schedule timestamp
* previous run
* conversation context
* model knowledge
* previous GitHub files
* examples

Use the current Vietnam calendar date to select EVEN or ODD mode.

Immediately before GitHub saving, capture the actual current Vietnam timestamp once.

Use that exact timestamp for both:

* filename
* commit message

Timestamp format:

`YYYY-MM-DD-HHmmss`

Example only:

`2026-08-18-123519`

Never reuse the example.

---

# 39. Output Delivery

Generate the complete final result as Markdown.

Do not create or attach a ChatGPT file.

Save the complete Markdown directly to GitHub using the available GitHub integration.

Repository:

`hakodev2k/Daily-OpenAI`

Directory:

`Daily Interview & Code Review Challenge`

Primary path:

`Daily Interview & Code Review Challenge/{RUN_TIMESTAMP}.md`

---

# 40. File Collision Handling

Before creating the file, check whether the path exists.

If absent:

`{RUN_TIMESTAMP}.md`

If it exists:

`{RUN_TIMESTAMP}-v2.md`

then:

`{RUN_TIMESTAMP}-v3.md`

and continue incrementally until an unused filename is found.

Never overwrite an existing challenge.

---

# 41. Git Commit

Use:

`Daily Interview & Code Review Challenge for {RUN_TIMESTAMP}`

The timestamp must be exactly the captured runtime timestamp.

If a filename collision requires `-v2`, `-v3`, etc., keep the original runtime timestamp in the commit message.

---

# 42. Repository History

When available, inspect recent files under:

`Daily Interview & Code Review Challenge/`

before generating the challenge.

Use them to support:

* skill rotation
* difficulty progression
* repetition prevention
* weakness revisiting
* CV skill coverage

Do not modify previous files.

Do not modify unrelated repository content.

---

# 43. Chat Output Rules

The complete challenge must exist only in the GitHub Markdown file.

Do not:

* preview questions in chat
* preview code-review snippets
* summarize the generated challenge in chat
* reproduce reference answers in chat
* reproduce the Markdown in chat

---

# 44. Final Response

If GitHub saving succeeds, return only:

`Saved to GitHub: Daily Interview & Code Review Challenge/{FINAL_FILENAME}`

If saving fails, return only:

`Error: [short reason the file could not be saved]`

Do not output the generated challenge after a save failure.