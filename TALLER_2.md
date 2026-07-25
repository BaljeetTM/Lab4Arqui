# Taller 2: Command Query Responsibility Segregation on Shortly

**Team:** 2 students

**Duration:** 19 june to 1 august.

**Final deliverable:** Modified codebase.

**Delivery format:**

- The work must be delivered via a project published on GitHub, where the commits of each team member must be reflected.
- If AI is used during development, **you must include** in the `IA.md` file the list of every prompt used.

---

## Requirements

* Clone the existing Shortly codebase from the provided GitHub repository.

### A. Must-Have — Core CQRS Implementation

1. **Separate Command and Query models**
   - **Goal:** Decouple write logic from read logic by introducing distinct models for commands and queries.
   - **Implementation:** Refactor the existing `Url` model into a **Command model** (used for creates, updates, deletes) and a **Query model / DTO** (used for reads). The controller/service layer must dispatch commands and queries through separate paths.
   - **Validation:** A write operation (e.g., `POST /api/urls`) must go through the command path, while a read operation (e.g., `GET /api/urls`) must go through the query path — no single model serving both purposes.
   - **Deliverable:** Distinct command and query model classes + updated endpoints.

2. **Command handlers**
   - **Goal:** Encapsulate each write operation in its own handler.
   - **Implementation:** Create dedicated command handler classes for `CreateUrlCommand`, `DeleteUrlCommand`, etc. Each handler receives the command and applies side effects (validation, persistence, event publishing).
   - **Validation:** Each write endpoint delegates to its corresponding command handler; handlers are independently testable.
   - **Deliverable:** Command handler classes + endpoint wiring.

3. **Query handlers**
   - **Goal:** Encapsulate each read operation in its own handler.
   - **Implementation:** Create dedicated query handler classes for `GetUrlQuery`, `ListUrlsQuery`, `GetStatsQuery`. Handlers return DTOs/ViewModels directly.
   - **Validation:** Each read endpoint delegates to its corresponding query handler; handlers are independently testable and return no side effects.
   - **Deliverable:** Query handler classes + endpoint wiring.

4. **Separate read / write repositories**
   - **Goal:** Isolate data access paths so reads never lock writes and vice versa.
   - **Implementation:** Create a **write repository** (e.g., `IUrlWriteRepository`) used only by command handlers, and a **read repository** (e.g., `IUrlReadRepository`) used only by query handlers. They may use separate `DbContext` instances or connection strings.
   - **Validation:** Command handlers inject the write repository; query handlers inject the read repository. No cross-use.
   - **Deliverable:** Two repository interfaces and implementations + DI registration.

5. **Read-optimized database schema**
   - **Goal:** Optimize the read side with denormalized tables or projections.
   - **Implementation:** Create a separate read-side table (e.g., `UrlReadModel`) that stores pre-joined / pre-computed data suitable for fast queries. The write side continues using the normalized schema.
   - **Validation:** Queries hit the read table; commands update the write table and synchronize the read table.
   - **Deliverable:** Read-model table definition + synchronization logic.

### B. Code Quality & Documentation

6. **Clean architecture & separation**
   - **Goal:** Ensure the project structure clearly separates commands, queries, and infrastructure.
   - **Implementation:** Organize code into folders or projects: `Commands/`, `Queries/`, `ReadRepositories/`, `WriteRepositories/`.
   - **Validation:** Navigating the codebase immediately reveals the CQRS structure.
   - **Deliverable:** Restructured project with clear boundaries.

7. **IA.md report**
   - **Goal:** Document AI usage if applicable.
   - **Implementation:** List every prompt used during development in `IA.md`.
   - **Deliverable:** `IA.md` file at repository root.

### C. Bonus Items

8. **CQRS performance report**
   - **Goal:** Quantify the read/write separation impact.
   - **Implementation:** Benchmark 3 endpoints (read-heavy, write-heavy, mixed) before and after CQRS. Measure throughput (req/s) and latency (p50/p90/p99).
   - **Validation:** Report shows before/after tables with comparable conditions.
   - **Deliverable:** `docs/benchmark-cqrs.md` with results and interpretation.

---

## Grading rubric

| # | Area                                 | Requirement(s)                                                                       | Points  |
|---|--------------------------------------|--------------------------------------------------------------------------------------|---------|
| 1 | **Command & Query models**           | Distinct command and query model classes; no single model serving both roles         | 15      |
| 2 | **Command handlers**                 | Dedicated handlers for each write operation; independently testable                  | 15      |
| 3 | **Query handlers**                   | Dedicated handlers for each read operation; return DTOs, no side effects             | 15      |
| 4 | **Separate read/write repositories** | Two repository interfaces/implementations; no cross-use between commands and queries | 20      |
| 5 | **Read-optimized schema**            | Read-model table with denormalized data; synchronization from write side             | 10      |
| 6 | **Code quality & architecture**      | Clean folder structure, clear separation of concerns                                 | 10      |
| 7 | **IA.md report**                     | AI prompts listed if AI was used                                                     | 5       |
|   | **Total (core)**                     |                                                                                      | **90**  |
| 8 | **CQRS performance report** (bonus)  | `docs/benchmark-cqrs.md` with before/after results                                   | 10      |
|   | **Total (with bonus)**               |                                                                                      | **100** |
