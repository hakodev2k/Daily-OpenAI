Generate today’s **Daily Developer Tips & Design** for the current run.

Do not create, modify, explain, or suggest an automation or schedule.

Do not review, summarize, rewrite, or discuss this prompt.

Do not ask follow-up questions.

Return only today’s completed lesson in Vietnamese while preserving technical terms, APIs, commands, code, product names, framework names, architectural terms, cloud services, and tool names in their original form.

---

# 1. Objective

Provide exactly **8 practical software-engineering insights each day**:

- exactly **5 Developer Tips & Tricks**
- exactly **3 Software Design topics**

The curriculum is tailored for a .NET-focused developer progressing from **Middle → Senior → Technical / Solution-oriented engineering capability**.

The lessons must progressively strengthen:

- everyday coding ability
- debugging and troubleshooting
- production engineering
- performance optimization
- database engineering
- cloud engineering
- distributed systems
- software architecture
- system design
- testing and quality
- DevOps and deployment
- frontend/backend integration
- security
- observability
- technical decision-making
- engineering trade-off analysis
- understanding of underlying mechanisms rather than memorizing syntax

Do not assume that familiarity with a technology means deep mastery.

When appropriate, explain the underlying reason a technique works, why a problem occurs, or what happens internally.

Avoid:

- generic motivational advice
- beginner trivia
- syntax-only lessons
- definition-only architecture content
- interview-question trivia with no production relevance

The content must be useful to an engineer working on real production systems.

---

# 2. Personal Technology Curriculum

Use the following curriculum as the main topic pool.

The goal is to progressively cover **all of these areas over time**, rather than repeatedly focusing only on .NET or architecture.

---

## A. C# & .NET Engineering

Cover topics including:

- C#
- modern C# language features
- .NET runtime
- .NET 6
- .NET 8
- .NET 10
- .NET Framework
- .NET Core migration
- CLR
- Garbage Collector
- memory allocation
- stack vs heap
- value types vs reference types
- records
- structs
- nullable reference types
- pattern matching
- generics
- delegates
- events
- expressions
- reflection
- attributes
- source generators
- dependency injection
- configuration
- Options Pattern
- middleware
- exception handling
- IDisposable / IAsyncDisposable
- Span
- Memory
- collections
- immutability
- serialization
- JSON handling
- asynchronous programming
- Task
- ValueTask
- async / await internals
- CancellationToken
- concurrency
- parallelism
- thread safety
- synchronization
- ThreadPool
- Channels
- BackgroundService
- HostedService
- scheduled jobs
- worker services

Prefer modern .NET 8 / .NET 10 techniques for new development.

Use .NET Framework, .NET Core 3.1 and older .NET versions mainly when discussing:

- legacy systems
- migration
- compatibility
- modernization
- technical debt
- upgrade strategy

---

## B. ASP.NET Core & Web Backend

Cover:

- ASP.NET Core
- ASP.NET MVC
- ASP.NET Web API
- RESTful APIs
- Minimal APIs where relevant
- middleware pipeline
- dependency injection
- model binding
- validation
- filters
- routing
- endpoint design
- API Versioning
- Swagger
- OpenAPI
- HTTP semantics
- HTTP caching
- response compression
- rate limiting
- authentication
- authorization
- JWT
- Azure AD
- Azure AD B2C / Microsoft Entra-related concepts
- cookies
- CORS
- CSRF
- API security
- file upload/download
- streaming
- pagination
- filtering
- sorting
- API error handling
- ProblemDetails
- API contracts
- backward compatibility
- API evolution
- third-party REST API integration
- retry strategies
- timeout handling
- webhook design
- idempotency

Include realistic Website, CMS, Mobile API and enterprise API scenarios.

---

## C. Entity Framework Core & Data Access

Cover:

- Entity Framework Core
- DbContext lifecycle
- change tracking
- AsNoTracking
- LINQ
- query translation
- IQueryable vs IEnumerable
- eager loading
- explicit loading
- projections
- N+1 queries
- transactions
- optimistic concurrency
- migrations
- batching
- bulk operations
- connection pooling
- database round trips
- Repository Pattern
- Unit of Work
- direct DbContext usage
- when Repository / Unit of Work adds value
- when these abstractions become unnecessary
- data access performance
- transaction boundaries

Avoid automatically treating Repository Pattern and Unit of Work as best practice.

Always consider their trade-offs against EF Core's built-in abstractions.

---

## D. Relational Databases

Cover both application-level usage and database internals.

Technologies:

- SQL Server
- PostgreSQL
- SQL

Topics:

- query execution plans
- indexes
- clustered / non-clustered concepts
- composite indexes
- covering indexes
- index selectivity
- index order
- statistics
- joins
- subqueries
- CTEs
- window functions
- pagination
- transactions
- isolation levels
- locking
- deadlocks
- blocking
- optimistic vs pessimistic concurrency
- ACID
- connection management
- query optimization
- schema design
- normalization
- denormalization
- constraints
- data types
- pagination strategies
- offset vs keyset pagination
- database performance troubleshooting
- slow queries
- index misuse
- over-indexing
- query plan analysis

Prefer explaining **why a query becomes slow**, not merely suggesting an index.

---

## E. NoSQL, Search & Caching

Technologies:

- MongoDB
- Cosmos DB
- Redis
- Elasticsearch

Topics may include:

### MongoDB / Cosmos DB

- document modeling
- partitioning
- partition keys
- hot partitions
- consistency models
- indexing
- query patterns
- denormalization
- RU / request-cost concepts where applicable
- data distribution
- scalability
- transactional limitations
- choosing NoSQL vs relational databases

### Redis

- cache-aside
- read-through concepts
- write-through concepts
- TTL
- eviction
- distributed cache
- cache invalidation
- cache stampede
- distributed locking
- session storage
- serialization
- stale data
- hot keys
- caching search results
- caching API responses

### Elasticsearch

- inverted indexes
- full-text search
- analyzers
- tokenization
- mappings
- filters vs queries
- relevance
- autocomplete
- geo search
- aggregations
- indexing strategies
- reindexing
- search performance
- synchronization with primary databases

Do not treat Elasticsearch as the primary transactional database unless the scenario explicitly warrants it.

---

## F. Microsoft Azure & Cloud Engineering

Cover:

- Microsoft Azure
- Azure App Service
- Azure Functions
- Azure Service Bus
- Azure Blob Storage
- Azure Monitor
- Application Insights
- Azure Key Vault
- Managed Identity
- Azure AD
- Azure AD B2C
- cloud configuration
- secrets management
- identity-based authentication
- networking concepts where relevant
- cloud deployment
- cloud scaling
- horizontal vs vertical scaling
- transient faults
- retry policies
- cloud cost awareness
- reliability
- availability
- serverless trade-offs
- application configuration
- distributed applications

### Azure Functions

Include:

- triggers
- bindings
- cold starts
- execution limits
- retry behavior
- idempotency
- poison messages
- background processing
- when Functions are appropriate
- when a normal worker service is better

### Azure Service Bus

Include:

- queues
- topics
- subscriptions
- dead-letter queues
- retries
- duplicate delivery
- at-least-once delivery
- message ordering
- sessions
- lock duration
- competing consumers
- idempotent consumers
- asynchronous workflows
- eventual consistency

### Azure Blob Storage

Include:

- file storage architecture
- SAS
- secure access
- Managed Identity
- metadata
- lifecycle management
- streaming large files
- direct client upload
- backend proxying trade-offs

---

## G. Distributed Systems & Integration

Cover:

- distributed systems fundamentals
- synchronous vs asynchronous communication
- REST vs messaging
- queues
- pub/sub
- message brokers
- Azure Service Bus
- eventual consistency
- CAP-related trade-offs when relevant
- distributed transactions
- Saga
- Outbox Pattern
- Inbox Pattern
- retry
- timeout
- circuit breaker
- backoff
- idempotency
- deduplication
- message ordering
- fault tolerance
- partial failure
- service boundaries
- third-party integration
- webhook reliability
- API integration failure handling
- compensating transactions

Use realistic production failure scenarios.

---

## H. Software Architecture & Design

Cover:

- Clean Architecture
- Onion Architecture
- Hexagonal Architecture
- Layered Architecture
- Modular Monolith
- Microservices
- Domain-Driven Design
- CQRS
- MediatR
- Repository Pattern
- Unit of Work
- Event-Driven Architecture
- SOLID
- DRY
- KISS
- YAGNI
- Separation of Concerns
- Dependency Inversion
- Dependency Injection
- Composition over Inheritance
- Tell Don't Ask
- Law of Demeter
- encapsulation
- cohesion
- coupling
- abstraction
- design patterns
- architectural boundaries
- bounded contexts
- domain models
- application services
- domain services
- infrastructure concerns
- technical debt
- evolutionary architecture

Architecture lessons must emphasize:

- why
- when
- when not
- cost
- operational impact
- team impact
- migration complexity
- failure modes

Never present an architecture as universally superior.

---

## I. Performance Engineering

Cover:

- application performance
- API latency
- database latency
- profiling
- allocations
- garbage collection
- async performance
- thread starvation
- connection pools
- database bottlenecks
- N+1 queries
- HTTP compression
- response compression
- caching
- CDN
- Akamai Cache
- browser caching
- server-side caching
- query optimization
- indexes
- memory usage
- CPU usage
- network overhead
- serialization overhead
- pagination
- streaming
- load testing
- bottleneck analysis
- latency percentiles
- throughput
- performance vs maintainability

Teach how to **measure before optimizing**.

Avoid premature micro-optimization.

---

## J. Observability, Logging & Production Troubleshooting

Technologies:

- Azure Monitor
- Application Insights
- Kibana
- Elasticsearch
- production monitoring

Topics:

- structured logging
- log levels
- correlation IDs
- distributed tracing
- metrics
- traces
- logs
- dashboards
- alerting
- dependency telemetry
- request telemetry
- exception telemetry
- latency analysis
- production debugging
- root-cause analysis
- incident investigation
- hotfix strategy
- deployment regression analysis
- troubleshooting memory issues
- troubleshooting CPU spikes
- troubleshooting slow APIs
- troubleshooting database issues
- troubleshooting external dependencies
- production support
- observability design

Prefer scenarios beginning with symptoms such as:

- API suddenly becomes slow
- CPU spikes
- errors increase after deployment
- database connections are exhausted
- message processing gets stuck
- cache hit ratio falls
- external API starts timing out

Then reason toward the cause.

---

## K. DevOps, Deployment & Infrastructure

Cover:

- Git
- Azure Repos
- CI/CD
- TeamCity
- Jenkins
- Octopus Deploy
- Azure App Service deployment
- IIS
- Windows Server
- Linux
- Kubernetes
- Docker where relevant
- release pipelines
- build pipelines
- deployment strategies
- environment configuration
- secrets
- rollback
- blue/green deployment
- rolling deployment
- feature flags
- health checks
- readiness
- liveness
- deployment failures
- artifact versioning
- branching strategies
- Git troubleshooting
- merge conflicts
- Git bisect
- production release practices

For Kubernetes, gradually cover:

- Pods
- Deployments
- Services
- ConfigMaps
- Secrets
- probes
- requests / limits
- autoscaling
- rolling updates
- pod failures
- container troubleshooting

Do not make Kubernetes dominate the curriculum.

---

## L. Testing & Code Quality

Cover:

- unit testing
- integration testing
- API testing
- end-to-end testing
- Playwright
- mocking
- test doubles
- contract testing
- database integration tests
- test architecture
- flaky tests
- test isolation
- deterministic tests
- test data
- regression testing
- production bug reproduction
- SonarQube
- static analysis
- code smells
- cyclomatic complexity
- code coverage
- mutation testing concepts where useful
- code review
- refactoring
- maintainability
- technical debt

Do not equate high code coverage with high software quality.

---

## M. Frontend & Full-Stack Engineering

Technologies and topics:

- JavaScript
- TypeScript
- Vue.js
- React.js
- Angular
- jQuery
- HTML5
- CSS3
- browser APIs
- HTTP
- frontend/backend integration
- state management
- component design
- async frontend code
- frontend performance
- API consumption
- authentication flows
- XSS
- CSRF
- CORS
- rendering
- browser caching
- bundle size
- network waterfalls
- frontend debugging

Prioritize:

1. Vue.js
2. React.js
3. JavaScript / TypeScript fundamentals

Use Angular and jQuery periodically for legacy-system understanding and maintenance scenarios.

---

## N. CMS, E-commerce & Content Platforms

Cover:

- Optimizely CMS
- CMS architecture
- content types
- page types
- blocks
- content modeling
- editorial workflows
- caching
- CMS APIs
- headless concepts
- composable commerce
- third-party integrations
- product systems
- authentication providers
- product reviews
- CMS performance
- content delivery
- integration boundaries
- CMS upgrade considerations

Use practical scenarios from enterprise CMS and e-commerce systems.

Do not make the curriculum Optimizely-specific; use Optimizely as an applied example of broader CMS concepts.

---

## O. Web Performance, SEO & Delivery

Cover:

- SEO engineering
- technical SEO
- server-side rendering concepts
- metadata
- canonical URLs
- redirects
- sitemap
- robots
- structured data
- Core Web Vitals
- Google Lighthouse
- accessibility
- image optimization
- compression
- CDN
- Akamai
- browser caching
- cache-control
- loading strategy
- JavaScript impact on performance
- backend latency impact on SEO
- website performance troubleshooting

Connect SEO topics to software engineering rather than digital-marketing advice.

---

## P. Developer Tools & Productivity

Cover practical usage of:

- Visual Studio
- Visual Studio Code
- Git
- Postman
- Swagger / OpenAPI
- Jira
- Figma when useful for developer collaboration
- browser DevTools
- database tools
- command line
- PowerShell
- Linux shell
- debugging tools
- profilers
- dotnet CLI
- developer productivity tools
- AI-assisted development
- AI coding agents
- code generation
- AI-assisted debugging
- AI-assisted code review

AI topics must focus on engineering quality.

Include concerns such as:

- hallucinated APIs
- incorrect assumptions
- insecure generated code
- missing edge cases
- architecture degradation
- excessive abstractions
- verification
- tests
- source inspection
- documentation grounding

Do not treat AI-generated code as automatically correct.

---

## Q. Engineering Process & Delivery

Cover:

- SDLC
- Agile
- Scrum
- Sprint Planning
- estimation
- requirement analysis
- technical solution analysis
- code review
- collaboration with QA
- collaboration with Product Owner / Product Manager
- production support
- release management
- incident response
- technical documentation
- requirement ambiguity
- technical risk identification
- breaking large features into deliverable units
- engineering trade-offs
- technical decision records

Focus on engineering judgment rather than project-management theory.

---

# 3. Curriculum Rotation Rules

The curriculum must be broad but controlled.

Before selecting today's topics, use recent Daily Developer Tips files in the repository, when available, to understand recent topic history.

Do not mention this history analysis in the output.

## Daily coverage

The 5 Developer Tips must cover at least **4 different curriculum groups**.

Do not make more than **2 Tips** about closely related technologies.

For example, avoid a day containing:

- 5 C# tips
- 5 database tips
- 5 Azure tips
- 5 debugging tips

At least one Tip should normally come from the developer's core stack:

- C#
- .NET
- ASP.NET Core
- EF Core
- SQL
- Azure

At least one Tip should normally come from a complementary area such as:

- Git
- testing
- frontend
- observability
- DevOps
- security
- debugging
- CMS
- production support
- developer tooling
- AI-assisted development

The 3 Software Design topics must cover at least **2 distinct design/system concerns**.

Do not make all 3 Design topics variants of the same architecture.

---

# 4. Long-Term Coverage Rules

Across consecutive runs, deliberately rotate among all curriculum groups.

Over approximately **7–14 runs**, ensure meaningful coverage across:

- C# / .NET
- ASP.NET Core / HTTP / APIs
- EF Core
- SQL Server / PostgreSQL
- MongoDB / Cosmos DB
- Redis / caching
- Elasticsearch / search
- Azure
- messaging / distributed systems
- architecture / DDD / CQRS
- performance
- production troubleshooting
- observability
- DevOps / deployment
- Kubernetes / infrastructure
- testing / code quality
- frontend
- security
- CMS / e-commerce
- web performance / SEO
- developer tools / Git
- AI-assisted development

Technologies do not need equal frequency.

Prioritize according to practical relevance:

### Highest frequency

- C#
- modern .NET
- ASP.NET Core
- EF Core
- SQL
- REST API
- Azure
- debugging
- production troubleshooting
- performance
- architecture
- testing
- Git

### Medium frequency

- Redis
- Cosmos DB
- MongoDB
- Azure Service Bus
- Azure Functions
- Kubernetes
- Elasticsearch
- Vue.js
- React.js
- CI/CD
- observability
- security
- CMS

### Periodic / context-driven

- Angular
- jQuery
- legacy .NET
- IIS
- Windows Server
- TeamCity
- Jenkins
- Octopus Deploy
- Akamai
- SEO
- Lighthouse
- Figma

Do not permanently omit lower-frequency technologies.
---

# 5. Repetition Control

Avoid repeating the same:

- exact tip
- API
- language feature
- design concept
- production scenario
- code example
- trade-off

too frequently.

If a topic reappears, increase depth.

For example:

First exposure:

`CancellationToken` basic usage.

Later exposure:

`CancellationToken` propagation across application layers.

Later:

cancellation with EF Core and outbound HTTP calls.

Later:

timeout vs cancellation in distributed systems.

Later:

cancellation behavior during application shutdown.

Likewise, a topic such as Redis may progress through:

cache-aside → TTL → invalidation → stampede → distributed cache consistency → operational failure scenarios.

Repeated topics should therefore create **depth progression**, not duplicate previous material.

---

# 6. Depth Progression

Gradually move lessons through these levels.

## Level 1 — Practical application

Examples:

- correct API usage
- useful debugging technique
- common production mistake
- useful framework feature

## Level 2 — Mechanism

Explain:

- why it works
- how the framework/runtime/database behaves
- what happens internally
- why the bug occurs

## Level 3 — Engineering judgment

Discuss:

- when to use it
- when not to
- alternatives
- operational consequences
- maintainability
- reliability
- performance
- complexity

## Level 4 — Senior / Technical reasoning

Discuss issues such as:

- architecture boundaries
- failure modes
- scalability
- transaction boundaries
- deployment concerns
- production diagnostics
- distributed consistency
- system evolution
- migration strategy
- team ownership
- technical debt
- cost of abstraction

Do not require every item to be Level 4.

The daily mix should remain concise and practical.

---

# 7. Part 1 — Developer Tips & Tricks

Provide exactly **5 practical Tips & Tricks**.

A Tip should preferably do one or more of the following:

- save development time
- prevent a common bug
- improve readability
- improve maintainability
- improve testability
- improve debugging
- help diagnose production problems
- improve performance
- improve reliability
- improve security
- expose an overlooked framework or language behavior
- improve development workflow
- reveal a useful diagnostic technique
- clarify a common misconception
- explain an important underlying mechanism

Whenever appropriate, prefer examples based on realistic systems such as:

- REST APIs
- CMS
- enterprise applications
- employee-management systems
- e-commerce
- event-management systems
- CRM
- search systems
- background jobs
- scheduled jobs
- Azure applications
- mobile backends
- third-party API integrations

---

## Tip format

Use exactly:

### Tip [1-5] — [Short title]

**Mẹo:**
Explain the tip briefly.

**Tại sao hữu ích:**
Explain the practical engineering benefit and, where relevant, the underlying reason.

**Ví dụ:**
Provide a small realistic example, code snippet, SQL query, command, log example, debugging scenario, architecture scenario, or production incident when useful.

**Lưu ý:**
Mention at least one common mistake, limitation, edge case, or situation where the technique should not be used.

Keep each Tip concise.

Do not turn a Tip into a long tutorial.

---

# 8. Part 2 — Software Design

Provide exactly **3 Software Design topics** suitable for Middle-to-Senior developers.

Possible domains include:

- software design principles
- SOLID
- DRY
- KISS
- YAGNI
- Separation of Concerns
- Composition over Inheritance
- Tell Don't Ask
- Law of Demeter
- Dependency Inversion
- encapsulation
- cohesion
- coupling
- design patterns
- DDD
- Clean Architecture
- Onion Architecture
- Hexagonal Architecture
- Modular Monolith
- Microservices
- Event-Driven Architecture
- CQRS
- Event Sourcing
- API design
- database design
- distributed systems
- caching
- messaging
- reliability
- scalability
- consistency
- concurrency
- resilience
- security design
- observability design
- testing strategy
- code architecture
- refactoring
- technical debt
- engineering trade-offs
- cloud architecture
- transaction boundaries
- integration architecture

Design topics must develop **engineering judgment**, not memorization.

Prefer situations involving:

- when to use something
- when not to use it
- trade-offs
- alternatives
- common misuse
- real production constraints
- maintainability
- performance
- reliability
- scalability
- operational complexity
- migration complexity
- team complexity
- system evolution

---

## Design format

Use exactly:

### Design [1-3] — [Concept]

**Khái niệm:**
Explain the concept clearly, but avoid a textbook-only definition.

**Vấn đề nó giải quyết:**
Describe the engineering problem or pressure that causes this design to become relevant.

**Ví dụ thực tế:**
Give a realistic software-development or production scenario.

**Trade-off:**
Explain at least one meaningful benefit and one meaningful cost, limitation, or risk.

**Middle → Senior insight:**
Clearly distinguish:

- what a Middle developer should understand or implement correctly
- what deeper judgment a Senior / Technical developer should demonstrate

Senior insight should often involve choosing between alternatives rather than simply knowing the pattern.

---

# 9. Design Difficulty Progression

Gradually include engineering decisions such as:

- DbContext directly vs Repository abstraction
- Modular Monolith vs Microservices
- REST vs messaging
- synchronous vs asynchronous communication
- SQL vs NoSQL
- MongoDB / Cosmos DB modeling
- Redis cache placement
- cache invalidation
- transaction boundaries
- database vs distributed transaction
- Outbox Pattern
- CQRS adoption
- DDD boundaries
- Event-Driven Architecture
- Azure Functions vs Worker Service
- Service Bus queue vs topic
- eventual consistency
- API backward compatibility
- API versioning
- idempotent APIs
- retry vs duplicate processing
- optimistic vs pessimistic concurrency
- database normalization vs query performance
- caching vs consistency
- abstraction vs unnecessary complexity
- generic framework vs business-specific implementation
- performance vs maintainability
- reliability vs implementation complexity
- Clean Architecture vs simpler layered architecture
- scaling application vs optimizing bottlenecks
- deployment simplicity vs infrastructure flexibility
- build vs buy
- synchronous integration vs asynchronous integration

Do not automatically recommend the most sophisticated design.

A Senior solution may be the simpler architecture when constraints justify it.

---

# 10. Production-Oriented Reasoning

Frequently use production scenarios.

Examples:

- API latency increases after deployment
- EF Core generates unexpectedly expensive SQL
- SQL query suddenly stops using an expected index
- Redis is unavailable
- cache contains stale data
- application instances process the same job twice
- Service Bus redelivers a message
- Azure Function retries unexpectedly
- App Service starts returning 5xx
- external API times out
- Kubernetes pod keeps restarting
- memory usage grows
- ThreadPool starvation occurs
- database connection pool is exhausted
- application logs are insufficient for debugging
- Elasticsearch data becomes inconsistent with the primary database
- CDN serves stale content
- authentication works locally but fails in production
- scheduled job runs on multiple application instances
- deployment succeeds but application health checks fail

Teach how to reason from:

**symptom → evidence → hypothesis → diagnosis → solution**

rather than immediately guessing a fix.

---

# 11. Security Coverage

Periodically include practical security topics such as:

- Authentication vs Authorization
- JWT validation
- token expiration
- refresh-token concepts
- Azure AD
- Azure AD B2C
- Managed Identity
- Key Vault
- secrets management
- CORS
- CSRF
- XSS
- SQL injection
- SSRF
- insecure deserialization
- input validation
- file-upload security
- API authorization
- least privilege
- secure logging
- sensitive-data exposure
- dependency vulnerabilities
- rate limiting
- secure third-party integrations

Keep security lessons relevant to application developers.

---

# 12. AI-Assisted Development Coverage

Periodically include advanced practical usage of AI tools for software engineering.

Topics can include:

- code generation
- refactoring
- debugging
- log analysis
- test generation
- architecture analysis
- documentation
- repository exploration
- migration assistance
- PR review
- SQL analysis
- production incident investigation
- prompt design for coding agents
- context management
- agent rules
- grounding against repository source
- validating generated code

Always emphasize verification.

Examples of failure modes:

- nonexistent APIs
- wrong library versions
- incorrect assumptions
- insecure code
- unnecessary abstractions
- copied architectural patterns without context
- generated tests that verify implementation instead of behavior
- SQL that works but performs poorly

The lesson should help an experienced developer use AI as an engineering accelerator rather than a substitute for technical judgment.

---

# 13. Quality Bar

Every item must pass these checks:

1. Is it useful in real engineering work?
2. Is it at least Middle-level relevant?
3. Does it explain more than syntax?
4. Is the example realistic?
5. Is there a meaningful pitfall or trade-off?
6. Could the developer realistically apply or recognize this at work?
7. Does it avoid unnecessary complexity?
8. Does it improve technical judgment?

If an item fails these checks, replace it before producing the lesson.

---

# 14. Variety Rules

Across consecutive runs, rotate among:

- language/runtime
- framework
- database
- caching/search
- cloud
- distributed systems
- debugging
- production support
- performance
- testing
- DevOps
- deployment
- security
- frontend
- CMS
- web performance
- developer tools
- architecture
- design principles
- AI-assisted development

Do not make every lesson about architecture.

Do not make every lesson about .NET.

Do not make every lesson about cloud.

Do not select obscure topics merely for novelty.

Prefer practical knowledge likely to occur in real software systems.

---

# 15. Output Structure

Return the lesson in this exact structure:

# Daily Developer Tips & Design — [Date]

## 🛠 Developer Tips & Tricks

### Tip 1 — ...

**Mẹo:**
...

**Tại sao hữu ích:**
...

**Ví dụ:**
...

**Lưu ý:**
...

### Tip 2 — ...

...

### Tip 3 — ...

...

### Tip 4 — ...

...

### Tip 5 — ...

...

## 🧠 Software Design

### Design 1 — ...

**Khái niệm:**
...

**Vấn đề nó giải quyết:**
...

**Ví dụ thực tế:**
...

**Trade-off:**
...

**Middle → Senior insight:**
...

### Design 2 — ...

...

### Design 3 — ...

...

---

# 16. Output Size

Keep the entire lesson practical and suitable for daily reading.

Prefer:

- concise explanations
- compact examples
- small code snippets
- focused trade-offs

Avoid long essays.

A code example should normally demonstrate only the specific point being taught.

---

# 17. Final Content Rules

Return exactly:

- 5 Developer Tips & Tricks
- 3 Software Design topics

Total: exactly **8 items**.

Do not add:

- extra tips
- bonus sections
- quizzes
- homework
- news
- career advice
- motivational content
- suggested learning actions
- conclusions
- recommended courses
- reading lists

---

# 18. Output Delivery

Generate the complete final result as Markdown.

Do not create or attach a ChatGPT file.

Save the complete Markdown content directly to GitHub using the available GitHub integration.

Repository:

`hakodev2k/Daily-OpenAI`

Directory:

`Daily Developer Tips & Design`

---

# 19. Runtime Timestamp

At the beginning of the run, obtain the **actual current date and time for Vietnam (UTC+7 / Asia/Ho\_Chi\_Minh)** using an available runtime/time tool.

Do not infer or guess the current time.

Do not derive the timestamp from:

- task schedule
- model knowledge
- conversation context
- previous run
- GitHub commit timestamp
- previous GitHub files
- examples in this prompt

The timestamp used for saving must represent the current run.

Capture the final runtime timestamp once immediately before saving.

Use that exact same timestamp for:

1. GitHub filename
2. GitHub commit message

Timestamp format:

`YYYY-MM-DD-HHmmss`

Example format only:

`2026-08-18-123519`

The example is illustrative only and MUST NOT be reused.

---

# 20. GitHub File Path

Primary target path:

`Daily Developer Tips & Design/{RUN_TIMESTAMP}.md`

Example structure:

`Daily Developer Tips & Design/2026-09-07-083015.md`

The timestamp above is only an example of formatting and MUST NOT be reused.

---

# 21. File Collision Handling

Before saving, check whether the target filename already exists.

If it does not exist:

`{RUN_TIMESTAMP}.md`

If it already exists, create:

`{RUN_TIMESTAMP}-v2.md`

If that exists:

`{RUN_TIMESTAMP}-v3.md`

Continue incrementally:

`-v4`, `-v5`, ...

until an unused filename is found.

Never overwrite an existing Daily Developer Tips file.

Use the final resolved filename in the success response.

---

# 22. Git Commit

Commit message:

`Daily Developer Tips & Design for {RUN_TIMESTAMP}`

The `{RUN_TIMESTAMP}` must be identical to the timestamp used in the filename before any `-vN` collision suffix.

---

# 23. GitHub Save Rules

The generated Markdown file is the primary deliverable.

Do not:

- paste the lesson into chat
- preview the lesson in chat
- summarize the lesson in chat
- reproduce any generated Tips in chat
- reproduce any generated Design topics in chat

The complete lesson must exist only in the saved GitHub Markdown file.

If necessary, inspect previous files only for:

- repetition avoidance
- curriculum rotation
- topic progression

Do not modify previous lesson files.

Do not modify unrelated repository files.

---

# 24. Final Chat Response

After the GitHub save succeeds, return only:

`Saved to GitHub: Daily Developer Tips & Design/{FINAL_FILENAME}`

Example format only:

`Saved to GitHub: Daily Developer Tips & Design/2026-09-07-083015.md`

Do not add any other text.

If the GitHub save fails, return only:

`Error: [short reason the file could not be saved]`

Do not output the generated lesson when saving fails.