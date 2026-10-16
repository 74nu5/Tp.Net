# Tp.Net — authoring guide

This repository holds the .NET onboarding labs for 3B Studio apprentices: short, hands-on half-day sessions (about 3 hours of exercises each), not deep-dive courses. The apprentice already programs (Java, Rust, Python, JS) but is new to .NET.

## Layout

- `index.html` — course home page (lab cards, setup checklist, language picker).
- `tp-XX-<slug>/enonce.html` — the lab statement, **in French**, as an interactive page. It links `../assets/enonce.css` and `../assets/enonce.js` and loads highlight.js 11.9.0 from cdnjs.
- `tp-XX-<slug>/README.md` — a short pointer to `enonce.html` plus the validation commands (GitHub does not render HTML).
- `tp-XX-<slug>/depart/` — starting point: compiles, contains `// TODO` markers and tests that fail until the exercises are done.
- `tp-XX-<slug>/solution/` — reference solution: all tests pass. Comments explain the choices.
- `tp-XX-<slug>/NOTES-FORMATEUR.md` — trainer notes, in French.
- Labs 02 → 04 build one "Tickets" app: each `depart/` starts from the previous `solution/` (copied, not referenced).
- `planning/plan-tp.html` — the curriculum planner; its embedded seed JSON is the source for lab titles, exercises and timings. Keep READMEs in sync with it.

## Statement markup (`enonce.html`)

The shared script turns plain markup into the interactive page; see the header comment of `assets/enonce.js`.

- `<body data-tp="tp-01" data-solution="depart/Katas.slnx">`, plus `data-test-project="depart/tests/<project>"` when the solution holds several test projects (otherwise the per-exercise filter fails with "Zero tests ran" on the others); side panel slots `#progress`, `#toc`, `#side-actions`, and `.compare-picker` (also allowed in the intro).
- One `<section class="exercise" id="exN" data-minutes="20" data-tests="Katas.Tests.ExerciseNN">` per exercise, starting with an `<h2>`; the script adds the number, duration, timer, "done" checkbox and the filtered `dotnet test` command. A section may carry its own `data-test-project` when its tests live in another project than the body's.
- Hints: `<div class="hints">` of `<details class="hint"><summary>topic</summary>…</details>`, unlocked one after the other. Answers: `<details class="answer">`.
- Comparisons: `<div class="bridge">` holding `<div class="compare">` with one `<pre data-lang="…">` per language: `csharp` (always shown), `java`, `rust`, `python`, `javascript`. Provide all five. Language-specific explanations use `<p data-lang="rust">…</p>`; table cells can carry `data-lang` too.
- `<section id="summary">` with `<div data-report>` and a `data-copy-report` button gives the time report.
- Escape `<`, `>` and `&` inside code. Check every non-C# snippet for correctness: the apprentice knows those languages.

## Conventions

- Code, identifiers, comments and technical docs in English; lab statements and trainer notes in French.
- Target `net10.0` (LTS). Only the C# 15 preview part of TP-05 targets `net11.0`, in its own folder with its own `global.json`.
- One `.slnx` per `depart/` and per `solution/`.
- Package versions live only in the root `Directory.Packages.props` (Central Package Management); never put `Version=` on a `PackageReference`.
- Database: SQL Server. Locally the apprentice uses their own instance; connection strings go in user-secrets, never in `appsettings.json`. Local connection strings need `TrustServerCertificate=True`.
- In `Program.cs`, read the connection string inside the `AddDbContext((services, options) => …)` lambda: `WebApplicationFactory` applies the test configuration after `Program.cs` has run.
- Integration tests that touch SQL Server go in the `Database` collection (`DatabaseFixture` recreates `TicketsTests` once per run). The test connection string comes from `testsettings.json`, overridden by `ConnectionStrings__TicketsTests` (set by CI).
- Tests use xUnit v3 on Microsoft.Testing.Platform (opted in by `global.json`): test projects are `OutputType=Exe`, no `Microsoft.NET.Test.Sdk`, and the command is `dotnet test --solution <file>.slnx`.
- NuGetAudit is on: fix a vulnerable transitive dependency with a pinned `PackageVersion` in `Directory.Packages.props` (transitive pinning is enabled).
- Self-validation is the point: every exercise should have an automated test the apprentice can turn green.
- Each exercise in a README has a duration in minutes; a half-day totals about 3 hours.

## Checks

- `dotnet build tp-XX-*/depart/*.slnx` must succeed (tests may fail).
- `dotnet test --solution tp-XX-*/solution/<file>.slnx` must pass.
