# Taller 1: HTTP Protocol Improvements on Shortly

**Team:** 2 students

**Duration:** 3 june to 23 july 2026.

**Final deliverable:** Modified codebase + benchmark report in `docs/`

**Delivery format:**

* The work must be delivered via a project published on GitHub, where the commits of each team member must be reflected.
* If AI is used during development, **you must include** in the `IA.md` file the list of every prompt used.

---

## Requirements

* Clone the existing Shortly codebase from the provided GitHub repository.

### A. Must-Have

1. **Hide time from ULID**
   - **Goal:** Prevent clients from inferring link creation time from public short codes.
   - **Implementation:** Replace direct ULID exposure with an irreversible representation (for example: hash/encoded token), while preserving uniqueness and URL-safe characters.
   - **Validation:** Generate multiple links and verify the output no longer contains recognizable ULID timestamp ordering patterns.
   - **Deliverable:** Updated ID generation code + comment/docstring explaining the privacy/security rationale.

### B. Core HTTP Protocol Improvements

2. **Response caching (`GET /{shortUrl}`)**
   - **Goal:** Reduce unnecessary redirect payload/processing using HTTP validators.
   - **Implementation:** Add `Cache-Control`, `ETag`, and `Last-Modified`; compute ETag from stable link state; handle `If-None-Match` and `If-Modified-Since`.
   - **Validation:** Send repeated requests with matching conditional headers and confirm `304 Not Modified` without body.
   - **Deliverable:** Redirect endpoint with conditional GET behavior + short note describing why `304` saves bandwidth.

3. **Security headers middleware**
   - **Goal:** Enforce baseline browser-side defenses on every HTTP response.
   - **Implementation:** Add middleware that sets `Strict-Transport-Security`, `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, and `Permissions-Policy` globally.
   - **Validation:** Verify headers exist on page, API, and redirect responses; confirm production-safe values.
   - **Deliverable:** Middleware + inline explanation of which attack type each header mitigates.

4. **Performance measurement middleware**
   - **Goal:** Make request latency observable for diagnostics and benchmarking.
   - **Implementation:** Measure request duration in middleware; append `X-Response-Time: <ms>ms`; emit dedicated slow-request logs for >500ms.
   - **Validation:** Confirm all responses include timing header and slow calls appear with method/path/status/elapsed.
   - **Deliverable:** Timing middleware + logging filter/query guidance for slow requests.

5. **Rate limiting with HTTP semantics**
   - **Goal:** Protect login endpoints from abuse using framework-level controls.
   - **Implementation:** Remove custom dictionary throttling from `UserService.Login`; configure built-in rate limiter policy and apply it at endpoint/controller level.
   - **Validation:** Trigger the limit and verify `429 Too Many Requests` with `Retry-After`.
   - **Deliverable:** Rate limiting configuration in `Program.cs` (or equivalent) + endpoint annotation/policy mapping.

6. **Compression (Brotli + Gzip)**
   - **Goal:** Reduce transfer size and improve response delivery time for compressible content.
   - **Implementation:** Enable `AddResponseCompression()` with Brotli and Gzip providers and configure MIME types safely.
   - **Validation:** Request with `Accept-Encoding` and confirm `Content-Encoding` plus reduced payload size.
   - **Deliverable:** Compression-enabled startup configuration + before/after size evidence.

7. **CORS with restrictive policy**
   - **Goal:** Allow only trusted client origins while blocking unauthorized cross-origin calls.
   - **Implementation:** Define explicit CORS policy (`origin`, `method`, `header`) and apply it with minimum scope needed.
   - **Validation:** Test both allowed and denied origins, including preflight `OPTIONS` requests.
   - **Deliverable:** CORS policy config + short documentation of preflight flow and observed headers.

8. **Content negotiation for errors**
   - **Goal:** Return standards-compliant, machine-readable error payloads.
   - **Implementation:** Replace plain-text redirect errors with `Results.Problem()` for `400`/`404`, including clear `title`, `status`, and `detail`.
   - **Validation:** Send requests with `Accept: application/problem+json` and verify RFC 7807 response shape/content type.
   - **Deliverable:** Updated endpoint error handling + short note on why problem details improve API interoperability.

9. **Cookie hardening audit**
   - **Goal:** Minimize session theft and cross-site abuse risks from cookies.
   - **Implementation:** Audit all cookie writes and enforce explicit options: `HttpOnly=true`, `SameSite=Strict`, `Path=/`, and `Secure=true` in production.
   - **Validation:** Inspect `Set-Cookie` headers and confirm every relevant cookie has hardened flags.
   - **Deliverable:** Hardened cookie configuration + documentation of each flag's security purpose.

10. **Conditional redirect status codes**
    - **Goal:** Align redirect responses with actual link permanence/temporality.
    - **Implementation:** Replace fixed `302` logic: return `301` for stable links with >100 accesses and `307` for temporary/expiring links.
    - **Validation:** Exercise both branches and verify status codes and redirect behavior in clients.
    - **Deliverable:** Redirect decision logic + explanation of semantic differences among `301`/`302`/`307`/`308`.

### C. Bonus Items

11. **HTTP/2 server push analysis + preload**
    - **Goal:** Evaluate modern asset delivery strategy for frontend performance.
    - **Implementation:** Analyze current assets (CSS/JS/images), discuss server push viability, and compare with preload hints.
    - **Validation:** Add `<link rel="preload">` for critical CSS and verify presence in final HTML output.
    - **Deliverable:** Short analysis document in `docs/` + implemented preload hint in layout/page template.

12. **Request tracing (`X-Request-Id`)**
    - **Goal:** Correlate all logs/events for a request path, including redirects.
    - **Implementation:** Add middleware to create or propagate a request ID, enrich Serilog context, and return `X-Request-Id` header.
    - **Validation:** Perform redirect flow and verify the same ID appears in request logs and response headers.
    - **Deliverable:** Trace middleware + logging enrichment configuration + brief usage note.

13. **Performance report (before/after)**
    - **Goal:** Quantify impact of protocol/middleware changes using repeatable benchmarks.
    - **Implementation:** Run `ab -n 1000 -c 10` on at least three endpoints before and after all changes.
    - **Validation:** Record requests/sec, p50/p90/p99, transfer rate, and failures; ensure same test conditions for both runs.
    - **Deliverable:** `docs/benchmark-before.md` and `docs/benchmark-after.md` with comparable tables and environment notes.

14. **Health check endpoint**
    - **Goal:** Provide a lightweight liveness endpoint for infrastructure probes.
    - **Implementation:** Enable ASP.NET Core Health Checks and expose `GET /health` with JSON status and uptime.
    - **Validation:** Confirm endpoint responds quickly with `200 OK` under normal operation.
    - **Deliverable:** Health endpoint wiring + response payload aligned with monitoring expectations.

15. **`robots.txt` and `sitemap.xml`**
    - **Goal:** Prevent indexing of shortener URLs while keeping crawler behavior explicit.
    - **Implementation:** Add `GET /robots.txt` with `Disallow: /`; add minimal/empty `GET /sitemap.xml`.
    - **Validation:** Verify response body and content types (`text/plain`, `application/xml`) for both endpoints.
    - **Deliverable:** Two crawler-control endpoints with clear intent documented in code comments.

---

## Grading rubric

| # | Area | Requirement(s) | Points |
|---|------|---------------|--------|
| 1 | **Hide time from ULID** | Item 1 (must-have) | 10 |
| 2 | **9 core HTTP improvements** | Items 2–10 (5 pts each) | 45 |
| 3 | **Benchmark report** | Item 13 | 20 |
| 4 | **Code quality + HTTP protocol reasoning** | All items in A and B | 15 |
| 5 | **Bonus items** | Items 11, 12, 14, 15 (2.5 pts each) | 10 extra |
| | **Total (without bonus)** | | **90** |
| | **Total (with bonus)** | | **100** |

**Note:** Each core modification (items 2–10) must include a **comment or docstring** in the code explaining the HTTP concept behind it (e.g., "ETag enables conditional `GET`: the client sends `If-None-Match` and we return `304` if unchanged, saving bandwidth").

**Classification summary:**
- **A. Must-Have (1 item):** #1
- **B. Core HTTP improvements (9 items):** #2–#10
- **C. Bonus (4 items):** #11, #12, #14, #15 — optional, up to 10 extra points
- **Benchmark report:** #13 — required (part of core deliverable)
