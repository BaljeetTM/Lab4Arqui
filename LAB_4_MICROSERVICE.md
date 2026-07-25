# Laboratory 4: Microservices Architecture with C4 Model

**Team:** 1 students

**Duration:** 12 june to 22 july 2026.

**Final deliverable:** C4 model diagrams + architecture document.

**Delivery format:**

- The work must be delivered via a project published on GitHub, where the commits of each team member must be reflected.
- If AI is used during development, **you must include** in the `IA.md` file the list of every prompt used.

---

## Requirements

* Clone the existing Shortly codebase from the provided GitHub repository.

### Objective

Design a microservices-based architecture for the Shortly URL shortener using the **C4 model** (Context, Container, Component, Code). The goal is to decompose the current monolithic system into independent, loosely coupled microservices, each with a single responsibility.

### A. C4 Diagrams

Create C4 diagrams for the Shortly app at the following levels:

1. **Level 1 — System Context diagram**
   - Show the Shortly system as a black box in the center.
   - Identify all external actors (e.g., end users, administrators, third-party services).
   - Identify external systems the Shortly system interacts with (e.g., databases, message brokers, monitoring services).
   - Label relationships with a brief description of the interaction.

2. **Level 2 — Container diagram**
   - Decompose Shortly into microservices (containers). Possible candidates:
     - **API Gateway** — single entry point, routes requests to appropriate services
     - **URL Service** — handles creation and management of short URLs
     - **Redirect Service** — handles `GET /{shortUrl}` redirects with high throughput
     - **Stats Service** — collects and serves click statistics
     - **Identity Service** — authentication and authorization (future)
     - Other services you consider necessary
   - Include data stores (databases, caches, message queues) each service uses.
   - Show communication protocols (HTTP/REST, gRPC, messaging, etc.).
   - Justify the chosen boundaries (why a particular responsibility lives in its own service).

3. **Level 3 — Component diagram**
   - Pick at least **2 microservices** and decompose them into internal components.
   - Show controllers, handlers, repositories, and any external dependencies.
   - Explain the internal structure and data flow for each selected service.

4. **Level 4 — Code (optional bonus)**
   - Provide a simplified class diagram or code snippet for one key component, showing the implementation-level structure.

### B. Diagram Format

Diagrams can be created using any of the following tools:

- **Structurizr DSL** (preferred — diagrams as code)
- **PlantUML** with C4 extension
- **Draw.io** / diagrams.net (export as PNG and include source file)
- Any other tool that supports the C4 notation

If using a graphical tool, commit both the source file and the exported image.

### C. Architecture Document

Alongside the diagrams, include a markdown document (`docs/architecture.md`) covering:

- **Service decomposition rationale** — why each service exists and its boundaries.
- **Communication patterns** — synchronous vs asynchronous, protocols used.
- **Data ownership** — which service owns which data; how data consistency is maintained.
- **Scalability considerations** — which services are expected to scale independently.
- **Failure modes** — what happens when a service is down; resiliency strategies.
- **Technology stack proposal** — suggested frameworks, databases, message brokers, etc.

---

## Grading rubric

| # | Area                               | Requirement(s)                                                                                        | Points  |
|---|------------------------------------|-------------------------------------------------------------------------------------------------------|---------|
| 1 | **Level 1 — Context diagram**      | System boundary, external actors, external systems correctly identified and labeled                   | 15      |
| 2 | **Level 2 — Container diagram**    | Microservices decomposed with clear boundaries; data stores shown; communication protocols indicated  | 25      |
| 3 | **Level 3 — Component diagrams**   | At least 2 services decomposed into internal components with data flow                                | 20      |
| 4 | **Architecture document**          | Clear rationale for decomposition, communication patterns, data ownership, scalability, failure modes | 20      |
| 5 | **Quality & completeness**         | Diagrams follow C4 notation; consistent styling; all relationships labeled                            | 10      |
| 6 | **IA.md report**                   | AI prompts listed if AI was used                                                                      | 10      |
|   | **Total**                          |                                                                                                       | **100** |
| 7 | **Level 4 — Code diagram** (bonus) | Class or code diagram for one key component                                                           | +5      |
