# Laboratory 5: Containerization with Docker

**Team:** 1 students

**Duration:** 22 june to 1 august 2026.

**Final deliverable:** Dockerfile + docker-compose.yml + documentation.

**Delivery format:**

- The work must be delivered via a project published on GitHub, where the commits of each team member must be reflected.
- If AI is used during development, **you must include** in the `IA.md` file the list of every prompt used.

---

## Requirements

* Clone the existing Shortly codebase from the provided GitHub repository.

### Objective

Containerize the Shortly web application using Docker. Write a production-ready `Dockerfile` and a `docker-compose.yml` that orchestrates the application along with its dependencies (database, cache, etc.).

### A. Dockerfile

Create a `Dockerfile` at the repository root with the following characteristics:

1. **Multi-stage build**
   - Stage 1 (build): Use the appropriate SDK image, restore dependencies, and publish the application.
   - Stage 2 (runtime): Use the smaller runtime image, copy the published output, and set the entry point.
   - Use `--no-restore` / `--no-build` flags where appropriate to avoid redundant work.

2. **Security best practices**
   - Run the application as a **non-root user** (e.g., `USER 1001` or `appuser`).
   - Do not expose the SDK or build tools in the final image.
   - Minimize the number of layers.

3. **Performance optimizations**
   - Leverage Docker layer caching by ordering `COPY` and `RUN` commands correctly (copy project files first, restore, then copy source code).
   - Set `ASPNETCORE_ENVIRONMENT=Production` in the image.
   - Expose the correct port (e.g., `80` or `8080`).

4. **Health check**
   - Add a `HEALTHCHECK` instruction that pings the `/health` endpoint (or a similar liveness probe).

### B. docker-compose.yml

Create a `docker-compose.yml` at the repository root that defines:

1. **Services**
   - `shortly-web` — builds from the `Dockerfile`, maps host port to container port.
   - `shortly-db` — database service (SQL Server, PostgreSQL, or SQLite depending on the current stack).
   - `shortly-cache` *(optional bonus)* — Redis or Memcached for caching frequently accessed URLs.

2. **Configuration**
   - Use environment variables or an `.env` file for configuration (connection strings, ports, etc.).
   - Set container names, restart policies, and resource limits (`deploy.resources`).

3. **Networks**
   - Define a custom bridge network for inter-service communication.
   - Do not expose the database port to the host unless strictly necessary.

4. **Volumes**
   - Use named volumes for persistent data (database files, etc.).
   - Use bind mounts for development scenarios (hot reload) if applicable.

### C. .dockerignore

Create a `.dockerignore` file that excludes unnecessary files from the build context:

- `node_modules/`, `bin/`, `obj/`, `.git/`, `.vs/`
- IDE and OS files (`.vscode/`, `.idea/`, `.DS_Store`, `Thumbs.db`)
- `*.md`, `*.log`, `docs/`

### D. Startup Instructions

Add or update a section in `README.md` (or create `docs/docker.md`) explaining:

- Prerequisites (Docker, Docker Compose versions).
- How to build and start the application: `docker compose up --build`.
- How to stop and clean up: `docker compose down -v`.
- How to view logs: `docker compose logs -f`.
- How to run a specific service: `docker compose run shortly-web`.

---

## Grading rubric

| # | Area | Requirement(s) | Points |
|---|------|----------------|--------|
| 1 | **Dockerfile** | Multi-stage build, non-root user, layer caching, HEALTHCHECK | 30 |
| 2 | **docker-compose.yml** | Web + database services, custom network, named volumes, env configuration | 30 |
| 3 | **.dockerignore** | Excludes build artifacts, IDE files, docs | 10 |
| 4 | **Security & best practices** | Non-root user, no SDK in final image, restart policies | 10 |
| 5 | **Documentation** | README or docs/docker.md with build, run, stop instructions | 10 |
| 6 | **IA.md report** | AI prompts listed if AI was used | 10 |
| | **Total** | | **100** |
| 7 | **Cache service** (bonus) | Redis or Memcached service in docker-compose + application integration | +5 |
