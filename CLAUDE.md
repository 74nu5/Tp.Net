# Tp.Net — authoring guide

This repository holds the .NET onboarding labs for 3B Studio apprentices: short, hands-on half-day sessions (about 3 hours of exercises each), not deep-dive courses. The apprentice already programs (Java, Rust, Python, JS) but is new to .NET.

## Layout

- `tp-XX-<slug>/README.md` — the lab statement, **in French**.
- `tp-XX-<slug>/depart/` — starting point: compiles, contains `// TODO` markers and tests that fail until the exercises are done.
- `tp-XX-<slug>/solution/` — reference solution: all tests pass. Comments explain the choices.
- `tp-XX-<slug>/NOTES-FORMATEUR.md` — trainer notes, in French.
- Labs 02 → 04 build one "Tickets" app: each `depart/` starts from the previous `solution/` (copied, not referenced).
- `planning/plan-tp.html` — the curriculum planner; its embedded seed JSON is the source for lab titles, exercises and timings. Keep READMEs in sync with it.

## Conventions

- Code, identifiers, comments and technical docs in English; lab statements and trainer notes in French.
- Target `net10.0` (LTS). Only the C# 15 preview part of TP-05 targets `net11.0`, in its own folder with its own `global.json`.
- One `.slnx` per `depart/` and per `solution/`.
- Package versions live only in the root `Directory.Packages.props` (Central Package Management); never put `Version=` on a `PackageReference`.
- Database: SQL Server. Locally the apprentice uses their own instance; connection strings go in user-secrets, never in `appsettings.json`. Local connection strings need `TrustServerCertificate=True`.
- Tests use xUnit v3 on Microsoft.Testing.Platform (opted in by `global.json`): test projects are `OutputType=Exe`, no `Microsoft.NET.Test.Sdk`, and the command is `dotnet test --solution <file>.slnx`.
- NuGetAudit is on: fix a vulnerable transitive dependency with a pinned `PackageVersion` in `Directory.Packages.props` (transitive pinning is enabled).
- Self-validation is the point: every exercise should have an automated test the apprentice can turn green.
- Each exercise in a README has a duration in minutes; a half-day totals about 3 hours.

## Checks

- `dotnet build tp-XX-*/depart/*.slnx` must succeed (tests may fail).
- `dotnet test --solution tp-XX-*/solution/<file>.slnx` must pass.
