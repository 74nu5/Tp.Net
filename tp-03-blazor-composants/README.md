# TP-03 · Blazor : composants, modes de rendu et formulaires

L'énoncé est interactif : ouvrez **[`enonce.html`](enonce.html)** dans un navigateur.

- `depart/` : l'API du TP-02, plus l'application Blazor à compléter (`depart/Tickets.slnx`).
- `solution/` : la correction commentée.
- `NOTES-FORMATEUR.md` : les notes pour animer la séance.

Lancer l'application : l'API (`dotnet run --project depart/src/Tickets.Api`, port 5080) puis le front (`dotnet run --project depart/src/Tickets.Web`, port 5090).

Valider le TP, depuis ce dossier :

```bash
dotnet build depart/Tickets.slnx                          # 0 avertissement
dotnet test --project depart/tests/Tickets.Web.Tests      # tests du front au vert
dotnet test --solution depart/Tickets.slnx                # et ceux de l'API (SQL Server requis)
```
