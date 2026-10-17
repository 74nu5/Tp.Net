/*
  Shared behaviour for the lab statements (tp-XX/enonce.html).

  Markup contract, written by hand in each statement:
    <body data-tp="tp-01" data-solution="depart/Katas.slnx" [data-test-project="depart/tests/…"]>
    <nav id="toc"></nav>, <div id="progress"></div>, <div id="side-actions"></div>   side panel slots
    <section class="exercise" id="ex2" data-minutes="15" data-tests="Katas.Tests.Exercise02">
      <h2>Title</h2> ...                                                              one card per exercise
      [data-test-project="depart/tests/…"] or [data-test-command="cd x &amp;&amp; dotnet test …"]       other project, or whole command
    <details class="answer"><summary>…</summary>…</details>                          an answer, open on demand
    <div class="bridge"><p data-lang="java">…</p><div class="compare">…</div></div>     comparison block; data-lang works on any element
    <div class="hints"><details class="hint">...</details>...</div>                   hints revealed in order
    <div class="compare"><pre data-lang="csharp">…</pre><pre data-lang="java">…</pre>…</div>       C# + chosen language side by side
    <p data-lang="rust">…</p>                                                         shown only when Rust is chosen
    <div class="compare-picker"></div>                                                language picker slot
    <section id="summary"><div data-report></div><button data-copy-report>…</button></section>   time report (title: document.title)

  Progress (done exercises, time spent) is stored in localStorage, per lab, in this browser only.
*/
(() => {
  "use strict";

  const body = document.body;
  const lab = body.dataset.tp || "tp";
  const solution = body.dataset.solution || "";
  // When a solution holds several test projects, filtering by namespace across all of them fails with
  // "Zero tests ran" on the others: data-test-project points the command to the lab's own test project.
  const testTarget = body.dataset.testProject ? `--project ${body.dataset.testProject}` : `--solution ${solution}`;
  const storageKey = `tpnet:${lab}:v1`;

  const exercises = [...document.querySelectorAll("section.exercise")];
  const totalMinutes = exercises.reduce((sum, ex) => sum + Number(ex.dataset.minutes || 0), 0);

  // ---------- State ----------
  let state = { done: {}, spent: {}, runningId: null, runningSince: 0 };
  try {
    const saved = JSON.parse(localStorage.getItem(storageKey) || "null");
    if (saved && typeof saved === "object") state = { ...state, ...saved };
  } catch (e) { /* storage unavailable: progress lives for this visit only */ }
  const save = () => { try { localStorage.setItem(storageKey, JSON.stringify(state)); } catch (e) { /* ignore */ } };

  const spentMs = id => (state.spent[id] || 0) + (state.runningId === id ? Date.now() - state.runningSince : 0);
  const fmtClock = ms => {
    const s = Math.floor(ms / 1000), h = Math.floor(s / 3600), m = Math.floor((s % 3600) / 60), r = s % 60;
    return (h ? h + ":" + String(m).padStart(2, "0") : String(m)) + ":" + String(r).padStart(2, "0");
  };
  const fmtMinutes = min => min >= 60 ? `${Math.floor(min / 60)} h ${String(min % 60).padStart(2, "0")}` : `${min} min`;

  function toast(message) {
    let el = document.querySelector(".toast");
    if (!el) { el = document.createElement("div"); el.className = "toast"; el.setAttribute("role", "status"); document.body.append(el); }
    el.textContent = message; el.hidden = false;
    clearTimeout(toast.timer); toast.timer = setTimeout(() => { el.hidden = true; }, 2200);
  }

  async function copyText(text, fallbackHost) {
    try {
      await navigator.clipboard.writeText(text);
      toast("Copié dans le presse-papiers");
    } catch (e) {
      if (!fallbackHost) return;
      let box = fallbackHost.querySelector(".copybox");
      if (!box) { box = document.createElement("textarea"); box.className = "copybox"; box.readOnly = true; fallbackHost.append(box); }
      box.value = text; box.focus(); box.select();
      toast("Sélectionné : copiez avec Ctrl+C");
    }
  }

  // ---------- Timer ----------
  function startTimer(id) {
    stopTimer();
    state.runningId = id; state.runningSince = Date.now(); save(); refresh();
  }
  function stopTimer() {
    if (!state.runningId) return;
    state.spent[state.runningId] = spentMs(state.runningId);
    state.runningId = null; state.runningSince = 0; save();
  }

  // ---------- Exercise cards ----------
  exercises.forEach((ex, index) => {
    const id = ex.id || (ex.id = `ex${index + 1}`);
    const title = ex.querySelector("h2");
    const minutes = Number(ex.dataset.minutes || 0);

    const head = document.createElement("div");
    head.className = "ex-head";
    const left = document.createElement("div");
    left.innerHTML = `<div class="num">Exercice ${index + 1}</div>`;
    title.replaceWith(head);
    left.append(title);
    const tools = document.createElement("div");
    tools.className = "ex-tools";
    tools.innerHTML = `
      <span class="chip planned" title="Durée prévue">${fmtMinutes(minutes)}</span>
      <button class="btn timer" type="button" data-timer="${id}" aria-label="Chronomètre de l'exercice ${index + 1}"></button>
      <label class="done-toggle"><input type="checkbox" data-done="${id}"> Terminé</label>`;
    head.append(left, tools);

    if (ex.dataset.tests || ex.dataset.testCommand) {
      const tests = document.createElement("div");
      tests.className = "tests";
      // An exercise can name its own test project (data-test-project on the section), e.g. API tests in a front-end lab.
      const target = ex.dataset.testProject ? `--project ${ex.dataset.testProject}` : testTarget;
      // data-test-command replaces the whole command, e.g. when it must run from a sub-folder with its own global.json.
      const command = ex.dataset.testCommand || `dotnet test ${target} --filter-namespace "${ex.dataset.tests}"`;
      tests.innerHTML = `<span class="lbl">Valider cet exercice, depuis le dossier du TP</span><pre><code class="language-bash"></code></pre>`;
      tests.querySelector("code").textContent = command;
      head.after(tests);
    }
  });

  // ---------- Side panel ----------
  const toc = document.getElementById("toc");
  if (toc) {
    toc.innerHTML = `<ul class="toc">${exercises.map((ex, i) => `
      <li data-for="${ex.id}"><a href="#${ex.id}">
        <span class="n"><span>${i + 1}</span></span>
        <span class="label">${ex.querySelector("h2").textContent}</span>
        <span class="t">${ex.dataset.minutes} min</span></a></li>`).join("")}</ul>`;
  }

  const sideActions = document.getElementById("side-actions");
  if (sideActions) {
    sideActions.className = "side-actions";
    sideActions.innerHTML = `<button class="btn ghost" type="button" data-reset>Réinitialiser ma progression</button>`;
  }

  // ---------- Code blocks: copy buttons and highlighting ----------
  document.querySelectorAll("pre").forEach(pre => {
    const button = document.createElement("button");
    button.type = "button"; button.className = "copy"; button.textContent = "Copier";
    button.addEventListener("click", () => copyText(pre.querySelector("code")?.textContent ?? pre.textContent, pre.parentElement));
    pre.append(button);
  });
  if (window.hljs) document.querySelectorAll("pre code[class*='language-']").forEach(code => window.hljs.highlightElement(code));

  // ---------- Comparison with another language ----------
  // C# is always shown. The apprentice picks the language to compare with; the choice is shared by every lab.
  const LANGUAGES = [
    { id: "java", label: "Java" },
    { id: "rust", label: "Rust" },
    { id: "python", label: "Python" },
    { id: "javascript", label: "JavaScript" },
    { id: "none", label: "Aucun" },
  ];
  const compareKey = "tpnet:compare-with";
  let compareWith = "java";
  try { compareWith = localStorage.getItem(compareKey) || compareWith; } catch (e) { /* keep the default */ }

  // Turn each <div class="compare"> of <pre data-lang> into a C# figure and a figure for the other language.
  const compares = [...document.querySelectorAll(".compare")].map(block => {
    const panes = new Map([...block.querySelectorAll(":scope > pre[data-lang]")].map(pre => [pre.dataset.lang, pre]));
    const wrap = (pre, label, cls) => {
      const figure = document.createElement("figure");
      figure.className = cls;
      figure.innerHTML = `<figcaption>${label}</figcaption>`;
      if (pre) figure.append(pre);
      return figure;
    };
    const csharp = wrap(panes.get("csharp"), "C#", "cs");
    const other = wrap(null, "", "other");
    block.replaceChildren(csharp, other);
    return { panes, other };
  });

  function applyLanguage() {
    const choice = LANGUAGES.find(lang => lang.id === compareWith) || LANGUAGES[0];
    compares.forEach(({ panes, other }) => {
      other.hidden = choice.id === "none";
      if (other.hidden) return;
      other.querySelector("figcaption").textContent = choice.label;
      other.querySelectorAll("pre, .missing").forEach(node => node.remove());
      const pane = panes.get(choice.id);
      if (pane) { other.append(pane); }
      else { const note = document.createElement("div"); note.className = "missing"; note.textContent = `Pas d'équivalent ${choice.label} pour cet exemple.`; other.append(note); }
    });
    // Explanations written for one language only.
    document.querySelectorAll("[data-lang]:not(pre)").forEach(node => {
      node.hidden = node.dataset.lang !== "csharp" && node.dataset.lang !== choice.id;
    });
    document.querySelectorAll(".picker button").forEach(button => button.setAttribute("aria-pressed", String(button.dataset.pick === choice.id)));
  }

  document.querySelectorAll(".compare-picker").forEach(slot => {
    slot.classList.add("picker");
    slot.innerHTML = `<div class="eyebrow">Comparer le C# avec</div>
      <div class="choices" role="group" aria-label="Langage de comparaison">${LANGUAGES.map(lang =>
        `<button type="button" data-pick="${lang.id}">${lang.label}</button>`).join("")}</div>`;
  });
  document.addEventListener("click", event => {
    const pick = event.target.closest("[data-pick]");
    if (!pick) return;
    compareWith = pick.dataset.pick;
    try { localStorage.setItem(compareKey, compareWith); } catch (e) { /* ignore */ }
    applyLanguage();
  });
  applyLanguage();

  // ---------- Hints revealed one after the other ----------
  document.querySelectorAll(".hints").forEach(group => {
    const hints = [...group.querySelectorAll(":scope > details")];
    hints.forEach((hint, i) => {
      const summary = hint.querySelector("summary");
      summary.textContent = `Indice ${i + 1} sur ${hints.length}` + (summary.textContent.trim() ? ` · ${summary.textContent.trim()}` : "");
      summary.addEventListener("click", event => {
        if (hint.getAttribute("aria-disabled") === "true") { event.preventDefault(); toast("Ouvrez d'abord l'indice précédent"); }
      });
      hint.addEventListener("toggle", lockHints);
    });
    // A hint unlocks once the previous one has been opened at least once.
    function lockHints() {
      hints.forEach(hint => { if (hint.open) hint.dataset.seen = "1"; });
      hints.forEach((hint, i) => {
        hint.setAttribute("aria-disabled", String(i > 0 && hints[i - 1].dataset.seen !== "1"));
      });
    }
    lockHints();
  });

  // ---------- Time report ----------
  const summary = document.getElementById("summary");
  function renderSummary() {
    if (!summary) return;
    const rows = exercises.map((ex, i) => {
      const spent = spentMs(ex.id);
      return `<tr><td>${i + 1}. ${ex.querySelector("h2").textContent}</td><td>${state.done[ex.id] ? "Terminé" : "—"}</td>
        <td>${ex.dataset.minutes} min</td><td>${spent ? fmtClock(spent) : "—"}</td></tr>`;
    }).join("");
    const total = exercises.reduce((sum, ex) => sum + spentMs(ex.id), 0);
    summary.querySelector("[data-report]").innerHTML = `
      <div class="table-wrap"><table class="summary-table">
        <thead><tr><th>Exercice</th><th>Statut</th><th>Prévu</th><th>Réel</th></tr></thead>
        <tbody>${rows}</tbody>
        <tfoot><tr><th>Total</th><th></th><th>${fmtMinutes(totalMinutes)}</th><th>${total ? fmtClock(total) : "—"}</th></tr></tfoot>
      </table></div>`;
  }
  function reportText() {
    const lines = exercises.map((ex, i) =>
      `${i + 1}. ${ex.querySelector("h2").textContent} : ${state.done[ex.id] ? "terminé" : "non terminé"}, prévu ${ex.dataset.minutes} min, réel ${spentMs(ex.id) ? fmtClock(spentMs(ex.id)) : "non chronométré"}`);
    return `${document.title}\n${lines.join("\n")}`;
  }

  // ---------- Refresh ----------
  function refresh() {
    const doneMinutes = exercises.reduce((sum, ex) => sum + (state.done[ex.id] ? Number(ex.dataset.minutes || 0) : 0), 0);
    const doneCount = exercises.filter(ex => state.done[ex.id]).length;
    const progress = document.getElementById("progress");
    if (progress) {
      progress.innerHTML = `
        <div class="eyebrow">Ma progression</div>
        <div class="progress-num">${doneCount} / ${exercises.length}</div>
        <div class="bar" role="img" aria-label="${doneCount} exercices terminés sur ${exercises.length}"><i style="width:${totalMinutes ? (doneMinutes / totalMinutes * 100).toFixed(1) : 0}%"></i></div>
        <div class="muted" style="font-size:.82rem">${fmtMinutes(doneMinutes)} sur ${fmtMinutes(totalMinutes)} · enregistré dans ce navigateur</div>`;
    }
    exercises.forEach(ex => {
      const done = !!state.done[ex.id];
      ex.classList.toggle("is-done", done);
      const box = ex.querySelector(`[data-done="${ex.id}"]`);
      if (box) box.checked = done;
      const li = toc && toc.querySelector(`[data-for="${ex.id}"]`);
      if (li) li.classList.toggle("done", done);
    });
    tick();
    renderSummary();
  }
  function tick() {
    exercises.forEach(ex => {
      const button = ex.querySelector(`[data-timer="${ex.id}"]`);
      if (!button) return;
      const running = state.runningId === ex.id;
      const spent = spentMs(ex.id);
      button.classList.toggle("running", running);
      button.textContent = running ? `⏸ ${fmtClock(spent)}` : spent ? `▶ ${fmtClock(spent)}` : "▶ Chrono";
      const chip = ex.querySelector(".chip.planned");
      if (chip) chip.classList.toggle("over", spent > Number(ex.dataset.minutes || 0) * 60000 * 1.25);
    });
  }
  setInterval(() => { if (state.runningId) { tick(); } }, 1000);

  // ---------- Events ----------
  document.addEventListener("change", event => {
    const id = event.target.dataset && event.target.dataset.done;
    if (!id) return;
    state.done[id] = event.target.checked;
    if (event.target.checked && state.runningId === id) stopTimer();
    save(); refresh();
    if (event.target.checked) toast("Exercice terminé");
  });
  document.addEventListener("click", event => {
    const timer = event.target.closest("[data-timer]");
    if (timer) { state.runningId === timer.dataset.timer ? (stopTimer(), refresh()) : startTimer(timer.dataset.timer); return; }
    const reset = event.target.closest("[data-reset]");
    if (reset) {
      if (reset.dataset.confirm !== "1") { reset.dataset.confirm = "1"; reset.textContent = "Confirmer la remise à zéro"; reset.classList.remove("ghost"); return; }
      state = { done: {}, spent: {}, runningId: null, runningSince: 0 }; save();
      reset.dataset.confirm = ""; reset.textContent = "Réinitialiser ma progression"; reset.classList.add("ghost");
      refresh(); toast("Progression remise à zéro"); return;
    }
    if (event.target.closest("[data-copy-report]")) copyText(reportText(), summary);
  });

  // Highlight the exercise being read in the side panel.
  if (toc && "IntersectionObserver" in window) {
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        toc.querySelectorAll("a").forEach(a => a.classList.toggle("current", a.getAttribute("href") === `#${entry.target.id}`));
      });
    }, { rootMargin: "-20% 0px -70% 0px" });
    exercises.forEach(ex => observer.observe(ex));
  }

  refresh();
})();
