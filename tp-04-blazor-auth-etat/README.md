# TP-04 · Blazor : authentification, état et composants réutilisables

L'énoncé est interactif : ouvrez **[`enonce.html`](enonce.html)** dans un navigateur.

- `depart/` : la solution du TP-03 (API et front Blazor), plus une page de connexion fournie et quelques squelettes à compléter (`depart/Tickets.slnx`).
- `solution/` : la correction commentée.
- `NOTES-FORMATEUR.md` : les notes pour animer la séance.

Lancer l'application : l'API (`dotnet run --project depart/src/Tickets.Api`, port 5080) puis le front (`dotnet run --project depart/src/Tickets.Web`, port 5090). Comptes de démonstration : `alice` / `demo` et `admin` / `demo`.

Valider le TP, depuis ce dossier :

```bash
dotnet build depart/Tickets.slnx                          # 0 avertissement
dotnet test --project depart/tests/Tickets.Api.Tests      # exercices 1 et 2 (SQL Server requis)
dotnet test --project depart/tests/Tickets.Web.Tests      # exercices 3 à 7
```
