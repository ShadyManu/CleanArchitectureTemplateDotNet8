# Clean Architecture Template (.NET 8)

A modern .NET 8 starter that uses **Clean Architecture** principles with a **custom CQRS** stack (no MediatR), custom **pipeline behaviors**, **command/query validation via `IValidatable`**, a **Result** pattern, and **minimal APIs organized with Carter**.  
It ships with **xUnit** unit tests and **integration tests with Testcontainers**.

---

## What it is

A practical, framework-agnostic template you can fork to build services or APIs with clear boundaries:

**Domain** → **Application** → **Infrastructure** → **Presentation**

Each layer is independent; cross-cutting concerns flow through a lightweight pipeline.  
The presentation surface is minimal (Carter modules), while business logic lives entirely in the Application & Domain layers.

---

## Core ideas & how it’s implemented

### 1) CQRS without MediatR

- Commands and Queries are simple request types with dedicated handlers.
- A custom **dispatcher** invokes a chain of **pipeline behaviors** (validation, logging) before executing the handler.
- No external mediator dependency — you own the primitives and lifecycle.

### 2) Validation with `IValidatable`

- Each Command/Query implements **`IValidatable`** and exposes a self-contained `Validate()` method.
- A **validation behavior** runs first in the pipeline; if validation fails, execution short-circuits and returns a failure `Result`.

### 3) Result pattern

- Handlers return `Result<T>` for **explicit success/failure**.
- Encourages consistent error modeling (messages/codes) and keeps endpoints thin: translate `Result` → HTTP.

### 4) Minimal endpoints with Carter

- **Carter modules** group endpoints by feature.
- Endpoints bind input → create Command/Query → dispatch → map `Result` to proper HTTP responses.
- Keeps the **Presentation layer** focused on routing/serialization.

### 5) Pipeline behaviors (custom)

- Central place for cross-cutting concerns (e.g., validation, logging) using **Decorator** pattern.
- Behaviors compose cleanly and are easy to test in isolation.

---

## 🧪 Testing strategy

- **Unit tests (xUnit)**
  - Validate rules on Commands/Queries first.
  - Test handlers separately, mocking their dependencies.
- **Integration tests (Testcontainers)**
  - Spin up real dependencies in Docker (e.g., MSSQL DB) and verify end-to-end flows through the minimal API (request → pipeline → handler → persistence).

This ensures fast feedback at the unit level and realistic coverage at the integration level.

---

## 🚀 Why this template

- Clean, opinionated defaults for **architecture & testing**.
- **Own your mediator**: simpler mental model, no magic, full control.
- **Predictable error handling** via `Result` everywhere.
- **Thin Presentation layer** and **portable Application** code.
- Container-friendly from day one (compose file & integration tests).

---

## 📁 Repository at a glance

- **Central Package Management:** `Directory.Packages.props`
- **Containers / orchestration:** `docker-compose.yml`, `docker-compose.override.yml`
- **Projects:**
  - `src/` — Domain, Application, Infrastructure, Presentation
  - `tests/` — xUnit projects (`Application.UnitTests` + `Infrastructure.IntegrationTests` with Testcontainers)
- **IDE/launch config:** `launchSettings.json`
- **Git/Docker hygiene:** `.gitignore`, `.dockerignore`

---

## ⚙️ Quick start (brief)

1. Open the solution in Visual Studio or Rider.
2. Adjust settings (connection strings, environment variables) — defaults work for local testing.
3. Run the project through **Docker Compose** so that the database spins up before the Presentation application starts.
4. Run the tests — **Unit first**, then **Integration** (Docker must be running for integration tests).

---

## 🤝 Contributing & License

Contributions are welcome — keep the architecture boundaries, pipeline, and testing approach consistent.  
See **LICENSE** for details.

---

**Author:** [Manuel Raso](https://github.com/ShadyManu)  
**LinkedIn:** [linkedin.com/in/manuelraso](https://www.linkedin.com/in/manuel-raso)  
**Repository:** [CleanArchitectureTemplateDotNet8](https://github.com/ShadyManu/CleanArchitectureTemplateDotNet8)
