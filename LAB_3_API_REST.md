# Laboratory 3: API REST implementation on Shortly

**Team:** 1 students

**Duration:** 3 to 18 july 2026.

**Final deliverable:** Modified codebase.

**Delivery format:**

* The work must be delivered via a project published on GitHub, where the commits of each team member must be reflected.
* If AI is used during development, **you must include** in the `IA.md` file the list of every prompt used.

---

## Requirements


* Clone the existing Shortly codebase from the provided GitHub repository.
* Implement a REST API for the Shortly system.
* The API must expose the functionality of the URL shortener through standard HTTP endpoints (GET, POST, DELETE, etc.) using JSON or XML depending on the request's `Accept` header (content negotiation).

**Minimum expected endpoints:**

- `POST /api/urls` — Create a shortened URL
- `GET /api/urls` — List all shortened URLs
- `GET /api/urls/{id}` — Get details of a specific shortened URL
- `DELETE /api/urls/{id}` — Delete a shortened URL
- `GET /api/stats` — Get usage statistics

**Authentication:** Not required for this delivery.

**Content negotiation:**

- The API must support both JSON and XML as response formats.
- The response format is selected based on the `Accept` request header.
- The request body format is determined by the `Content-Type` header.
- If `Accept: application/json` → respond with JSON.
- If `Accept: application/xml` or `text/xml` → respond with XML.
- If `Accept` is absent or `*/*` → default to JSON.
- If the client does not accept any supported format → return `406 Not Acceptable`.

---

## Grading rubric

| # | Area                               | Requirement(s)                                                                                             | Points  |
|---|------------------------------------|------------------------------------------------------------------------------------------------------------|---------|
| 1 | **REST API endpoints**             | All 5 endpoints implemented and working correctly                                                          | 50      |
| 2 | **Content negotiation (JSON/XML)** | Response format selected via `Accept` header; valid JSON and XML produced; `Content-Type` parsed for input | 20      |
| 3 | **HTTP methods and status codes**  | Correct use of GET, POST, DELETE and proper status codes (200, 201, 204, 404, 406, etc.)                   | 15      |
| 4 | **Code quality & error handling**  | Clean code, consistent structure, proper error responses                                                   | 10      |
| 5 | **IA.md report**                   | AI prompts listed if AI was used during development                                                        | 5       |
|   | **Total**                          |                                                                                                            | **100** |
